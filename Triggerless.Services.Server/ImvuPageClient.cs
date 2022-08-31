using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Models;
using log4net;

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
            }



            return result;

        }

        public void Dispose()
        {
            _service?.Dispose();
        }
    }
}
