using NAudio.Vorbis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Triggerless.API.Models;
using Triggerless.Models;
using Triggerless.Services.Server;


namespace Triggerless.Services.Client
{
    public class ImvuContentApiClent: IDisposable
    {
        private ImvuContentApiService _service;

        

        public ImvuContentApiClent()
        {
            _service = new ImvuContentApiService();
        }

        public async Task<XmlDocument> GetIndexXml(long productId)
        {
            try
            {
                var xmlStr = await _service.GetString($"{productId}/1/index.xml");
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStr);
                return xmlDoc;
            }
            catch 
            {
                return null;
            }
        }

        public async Task<ContentsJsonItem[]> GetContentJsonItems(long productId)
        {
            try
            {
                var items = await _service.GetPoco<ContentsJsonItem[]>($"{productId}/1/index.xml");
                return items;
            }
            catch
            {
                return new ContentsJsonItem[0];
            }
        }

        private async Task<double> GetOggLengthMS(long productId, string oggLocation)
        {
            var url = $"{productId}/1/{oggLocation}";
            int tryCount = 0;
            const int TRY_MAX = 10;
            const int TIMEOUT = 1000;

            while (tryCount < TRY_MAX) 
            {
                try
                {
                    using (var net = await _service.GetStream(url))
                    using (var ms = new MemoryStream())
                    {
                        await net.CopyToAsync(ms);
                        ms.Position = 0; // rewind before reading

                        using (var vorb = new VorbisWaveReader(ms, false)) // don't close ms automatically
                        {
                            return vorb.TotalTime.TotalMilliseconds;
                        }
                    }
                }
                catch (Exception ex)
                {
                    tryCount++;
                    await Task.Delay(TIMEOUT);
                }
            }
            return -1 * tryCount;
        }

        public async Task<CollectorResponsePayload> GetOggLengthsMS(CollectorPayload payload)
        {
            var dbClient = new BootstersDbClient();
            var existing = await dbClient.LookupProductTriggers(payload);
            if (existing.Triggers.Any())
            {
                return existing;
            }


            var result = new CollectorResponsePayload
            {
                ProductId = payload.ProductId,
                CreatorName = payload.CreatorName,
                ImageBytes = payload.ImageBytes,
                ImageLocation = payload.ImageLocation,
                DateCreated = DateTime.UtcNow,
                AddedBy = payload.AddedBy,
                ProductName = payload.ProductName,
                Message = "All triggers successful",
                Result = ScanResultType.Success
            };
            //result.Triggers.AddRange(payload.Triggers); do this last




            var processorCount = Environment.ProcessorCount;
            int maxThreads = 2 * processorCount / 3;
            var semaphore = new SemaphoreSlim(maxThreads);
            bool successAll = true;

            var tasks = payload.Triggers.Select(async trigger =>
            {
                var tryCount = 0;
                var tryMax = 10;
                await semaphore.WaitAsync();
                try
                {
                    var start = DateTime.Now;

                    using (var triggerClient = new ImvuContentApiClent())
                    {
                        bool bSuccess = false;
                        while (tryCount < tryMax)
                        {
                            try
                            {                                
                                trigger.LengthMS = await triggerClient.GetOggLengthMS(trigger.ProductId, trigger.Location);
                                trigger.WaitMS = (DateTime.Now - start).TotalMilliseconds;
                                bSuccess = true;
                                break;
                            }
                            catch (Exception exc)
                            {
                                tryCount++;
                                var msg = ($"  **TRIGGER GET failed Try {tryCount}: {payload.ProductName} {trigger.OggName} {trigger.TriggerName} \n{exc.Message}\n");
                                result.Message = msg;
                                await Task.Delay(50 * tryCount);
                            }
                        }
                        if (!bSuccess)
                        {
                            /*
                            string ouch = $"Unable to read trigger {trigger.TriggerName} for {product.ProductName} (pid = {product.ProductId}) after {tryMax} tries";
                            _ = await Discord.SendMessage("Scan Failure", ouch).ConfigureAwait(false);
                            LogLine($"  !!OUCH: {ouch}");
                            successAll = false;
                            */
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    var msg = $"  **TRIGGER GET failed on last try {payload.ProductName} {trigger.TriggerName} {trigger.OggName}";
                    //LogLine(msg);
                    _ = await Discord.SendMessage("Trigger Download Failure", msg);
                }
                catch (Exception exc)
                {
                    //LogLine($"**ERROR: {product.ProductName} {exc.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToArray();

            await Task.WhenAll(tasks);


            if (!successAll)
            {
                result.Result = ScanResultType.NetworkError;
                result.Message = "At least one trigger could not be downloaded.";
                return result;
            }
            else
            {
                result.Triggers.AddRange(payload.Triggers);
                var save = await dbClient.SaveProductTriggers(result.Triggers);
                if (save.Result != ScanResultType.Success)
                {
                    result.Result = save.Result;
                    result.Message = save.Message;
                }
            }
            return result;
        }

        public void Dispose()
        {
            _service.Dispose();
            _service = null;
        }
    }
}
