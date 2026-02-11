using log4net;
using Newtonsoft.Json.Linq;
using Polly.Caching;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Triggerless.Models;

namespace Triggerless.Services.Server
{
    public class ImvuPageClient: IDisposable
    {
        private ImvuPageService _service;
        private ILog _log;

        public ImvuPageClient(ILog log = null)
        {
            _log = log;
            _service = new ImvuPageService();
        }

        public async Task<string> GetAvatarCardJson(long id)
        {
            try {
                var json = await _service.GetJsonString($"http://www.imvu.com/api/avatarcard.php?cid={id}");
                var jobject = JObject.Parse(json);
                if (jobject == null) return $"\"status\": \"failure\", \"message\": \"Malformed JSON returned from IMVU\"";
                if (jobject["error"] != null && jobject["error"].Value<string>() != null)
                {
                    return $"\"status\": \"failure\", \"message\": \"No Avatar information for CID {id}\"";
                }
                return json;
            } 
            catch (Exception exc)
            {
                return $"{{\"status\": failure, \"message\": \"{exc.Message}\"}}";
            }
        }

        public async Task<ImvuProduct> GetHiddenProduct(long productId)
        {
            var result = new ImvuProduct { 
                Id = productId,
                Name = "Unknown",
                CreatorName = "Unknown",
                ProductImage = "Unknown",
                CreatorId = 0
            };
            result.IsVisible = false;

            var lines = await _service.GetLines($"shop/product.php?products_id={productId}");

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("</title>"))
                {
                    int iLeft = 14;
                    int iRight = lines[i].IndexOf(" by ", iLeft);
                    if (iRight == -1) return result;
                    result.Name = lines[i].Substring(iLeft, iRight - iLeft);

                    iLeft = iRight + 4;
                    iRight = lines[i].IndexOf("</title>", iLeft);
                    result.CreatorName = lines[i].Substring(iLeft, iRight - iLeft);
                }

                if (lines[i].Contains("<meta property=\"og:image\""))
                {
                    int iLeft = 35;
                    int iRight = lines[i].IndexOf("\"", iLeft);
                    if (iRight == -1) return result;
                    result.ProductImage = lines[i].Substring(iLeft, iRight - iLeft);
                    break;
                }
            }

            using (var apiClient = new ImvuApiClient())
            {
                var user = await apiClient.GetUserByName(result.CreatorName);
                result.CreatorId = user.Id;

                var template = await apiClient.GetTemplate(productId);
                result.ParentId = template.ParentProductID;
            }





            return result;

        }

        public async Task<List<ImvuProduct>> GetDerivedProducts(long productId)
        {
            var result = new List<ImvuProduct>();

            var page = 1;
            var count = 0;
            var pidList = new List<long>();
            do
            {
                var pageUrl = $"/shop/web_search.php?derived_from={productId}&page={page}";
                var html = await _service.GetString(pageUrl);
                var pattern = @"product-index-\d+"" id=""(?<pid>\d+)"""; var matches = Regex.Matches(html, pattern);
                foreach (Match m in matches)
                {
                    if (m.Success) pidList.Add(long.Parse(m.Groups["pid"].Value));
                }
                count = matches.Count;
                page++;
            } while (count > 0);

            // PARALLEL (max 16 concurrent), with pre-materialized IDs
            var productDict = new ConcurrentDictionary<long, ImvuProduct>();

            // Coalesce enumeration before starting any tasks
            var ids = pidList.Distinct().ToList(); // Distinct() optional

            using (var apiClient = new ImvuApiClient())
            {
                var sem = new SemaphoreSlim(16);
                var tasks = new List<Task>(ids.Count);

                foreach (var pid in ids)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await sem.WaitAsync().ConfigureAwait(false);
                        try
                        {
                            var product = await apiClient.GetProduct(pid).ConfigureAwait(false);
                            if (product != null)
                                productDict[product.Id] = product;
                        }
                        catch
                        {
                            // TODO: log if needed
                        }
                        finally
                        {
                            sem.Release();
                        }
                    }));
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            // END PARALLEL

            result = productDict
                .Values
                .OrderByDescending(v => v.Id)
                .ToList();
            return result;
        }
        public void Dispose()
        {
            _service?.Dispose();
        }
    }
}
