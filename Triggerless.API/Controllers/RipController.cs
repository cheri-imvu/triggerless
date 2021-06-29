using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Newtonsoft.Json;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class RipController : BaseController {


        [Route("api/Rip/{pid}")]
        public async Task<HttpResponseMessage> Get(int pid) {

            HttpResponseMessage response;
            string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (pid != 32678253)
            {
                bool home = ipAddress == (ConfigurationManager.AppSettings["homeIP"] ?? "98.201.77.24");
                bool local = ipAddress == "127.0.0.1";
                bool localnet = ipAddress.StartsWith("192.168.");
                bool mynet = ipAddress == "143.95.252.34";

                if (!home && !local && !localnet && !mynet)
                {
                    response = new HttpResponseMessage(HttpStatusCode.Forbidden) {Content = new StringContent("Access Denied") };
                    return response;
                }

            }

            try {
                BootstersDbClient.SaveRipInfo(pid, ipAddress, DateTime.UtcNow);
                var bytes = await GetFileBytes(pid);
                response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new ByteArrayContent(bytes)};
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition =
                    ContentDispositionHeaderValue.Parse($"attachment; filename = \"product-{pid}.chkn\"");
                return response;

            } catch (FileNotFoundException) {
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return response;
            } catch (Exception) {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                return response;
            }
        }

        private async Task<byte[]> GetFileBytes(int pid) {
            // ReSharper disable once AssignNullToNotNullAttribute
            string target = Path.Combine(HostingEnvironment.MapPath("/"), $@"Rips");
            if (!Directory.Exists(target)) Directory.CreateDirectory(target);

            using (var client = new HttpClient())
            {
                //client.Headers.Add(HttpRequestHeader.KeepAlive, "true");
                var urlTemplate = $"http://userimages-akm.imvu.com/productdata/{pid}/1/{{0}}";
                //client.Timeout = new TimeSpan(0, 1, 0);

                var url = string.Format(urlTemplate, "_contents.json");
                var responseJson = await client.GetStringAsync(url);
                var json = $"{{productArray: {responseJson}}}";

                var list = JsonConvert.DeserializeObject<ProductList>(json);
                target = Path.Combine(target, pid.ToString());
                if (!Directory.Exists(target)) Directory.CreateDirectory(target);

                foreach (ProductItem item in list.productArray)
                {

                    url = string.Format(urlTemplate, item.url ?? item.name);
                    item.content = await client.GetByteArrayAsync(url);
                    item.length = item.content.Length;
                    Console.WriteLine(item.name);
                    File.WriteAllBytes(Path.Combine(target, item.name), item.content);
                }

                var zipPath = Path.Combine(target, $"product-{pid}.chkn");

                using (var fs = new FileStream(zipPath, FileMode.Create))
                {
                    using (ZipArchive za = new ZipArchive(fs, ZipArchiveMode.Create, false))
                    {
                        foreach (ProductItem item in list.productArray)
                        {
                            var itemPath = Path.Combine(target, item.name);
                            za.CreateEntryFromFile(itemPath, item.name, CompressionLevel.Fastest);
                            File.Delete(itemPath);
                        }
                    }
                }

                var fileBytes = File.ReadAllBytes(zipPath);
                File.Delete(zipPath);
                Directory.Delete(target);

                return fileBytes;
           }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        public class ProductList
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public ProductItem[] productArray { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        public class ProductItem
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public string url { get; set; }
            public string name { get; set; }
            public string original_dimensions { get; set; }
            public string[] tags { get; set; }
            public byte[] content { get; set; }
            public long length { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }


    }
}