using NAudio.Vorbis;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Triggerless.Models;
using Triggerless.Services.Client;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{

    [RoutePrefix("api/bot")]
    public class TriggerbotController : CustomerController
    {
        [Route("event/{eventId}"), HttpPost]
        public async Task<IHttpActionResult> RecordEvent(short eventId)
        {
            var jsonText = await Request.Content.ReadAsStringAsync(); // raw body

            IHttpActionResult result = Ok("Nothing yet");
            var location = "Internal Network";
            var ip = GetClientIp();
            if (ip != UNKNOWN_IP && ip != "127.0.0.1")
            {
                try
                {
                    location = await GetLocationFromIpAsync(ip);
                }
                catch (Exception)
                {
                    // don't stop for this burp
                    location = "Unknown Location";
                }
            }
            var cid = CustomerID ?? 0;
            BootstersDbClient.EventType eventType = (BootstersDbClient.EventType)eventId;

            var dbClient = new BootstersDbClient();
            BootstersDbClient.EventResult dbResult = null;
            Exception exception = null;
            try
            {
                dbResult = await dbClient.SaveEventAsync(eventType, cid, jsonText);
            }
            catch (Exception ex) 
            { 
                dbResult = new BootstersDbClient.EventResult() { 
                    Message = ex.Message, 
                    Type = BootstersDbClient.EventResultType.Fail 
                };
                exception = ex; 
            }

            if (dbResult.Type == BootstersDbClient.EventResultType.Success)
            {
                result = Ok(dbResult);
            }
            else {
                result = InternalServerError(exception);
            }
            return result;
        }

        [Route("testa"), HttpGet()]
        public async Task<IHttpActionResult> TestAkamai()
        {
            byte[] bytes = new byte[0];
            using (var http = new HttpClient()) 
            {
                var url = "http://userimages02-akm.imvu.com/productdata/images_a7964c4c0e9565ba8eb8dcc8029a213d.png";
                try
                {
                    bytes = await http.GetByteArrayAsync(url);
                }
                catch (Exception exc)
                {
                    return InternalServerError(exc);
                }
            }
            return Ok(bytes.Length);
        }

        [Route("ogg/{pid}/{filename}"), HttpGet()]
        public async Task<IHttpActionResult> TestOgg(long pid, string filename)
        {
            DateTime start = DateTime.Now;
            using ( var http = new HttpClient())
            {
                var url = $"https://userimages-akm.imvu.com/productdata/{pid}/1/{filename}.ogg";
                try
                {
                    using (var net = await http.GetStreamAsync(url))
                    using (var ms = new MemoryStream())
                    {
                        await net.CopyToAsync(ms);
                        ms.Position = 0; // rewind before reading

                        using (var vorb = new VorbisWaveReader(ms, false)) // don't close ms automatically
                        {
                            var wait = (DateTime.Now - start).TotalMilliseconds;
                            return Ok(new
                            {
                                Milliseconds = vorb.TotalTime.TotalMilliseconds,
                                Wait = wait
                            });
                        }
                    }
                }
                catch (Exception exc)
                { 
                    return InternalServerError(exc);
                }
            }
        }

        [Route("ogg/lengths"), HttpPost()]
        public async Task<CollectorResponsePayload> GetOggLengths(CollectorPayload payload)
        {
            var result = new CollectorResponsePayload();
            using (var client = new ImvuContentApiClent())
            {
                result = await client.GetOggLengthsMS(payload);
            }
            return result;
        }
    }
}
