using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Newtonsoft.Json;
using Triggerless.Models;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class RipController : BaseController {

        public virtual string AllowedIPs
        {
            get
            {
                return (ConfigurationManager.AppSettings["allowedIPs"]);
            }
        }

        public virtual string DeniedIPs
        {
            get
            {
                return (ConfigurationManager.AppSettings["deniedIPs"]);
            }
        }

        public virtual string RemoteIP
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }

        public virtual bool RestrictIPs
        {
            get
            {
                string restricted = ConfigurationManager.AppSettings["restrictIPs"];
                if (restricted?.ToLower() == "false" || restricted?.ToLower() == "no") return false;
                return true;
            }
        }

        [Route("api/Rip/{pid}")]
        public async Task<HttpResponseMessage> Get(int pid) {

            HttpResponseMessage response;
            string ipAddress = RemoteIP;

            // Denied IPs are always denied, regardless of RestrictIPs

            char[] semi = ";".ToCharArray();
            var deniedAddresses = DeniedIPs.Split(semi, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            var deniedMessage = new HttpResponseMessage(HttpStatusCode.Forbidden)
                { Content = new StringContent("Access Denied to '{ipAddress}'. Contact avatar @Triggers to negotiate an access fee.") };
            if (deniedAddresses.Contains(ipAddress))
            {
                return deniedMessage;
            }


            if (RestrictIPs)
            {
                var allowedAddresses = AllowedIPs.Split(semi, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                bool allowed = false;


                foreach (var ip in allowedAddresses)
                {
                    if (ip.EndsWith("*"))
                    {
                        allowed |= ipAddress.StartsWith(ip.Replace("*", ""));
                    }
                    else
                    {
                        allowed |= ipAddress == ip;
                    }

                }

                allowed |= ipAddress == "127.0.0.1" || ipAddress == "::1";
                allowed |= ipAddress.StartsWith("192.168.");


                if (!allowed)
                {
                    return deniedMessage;
                }
            }

            try {
                new BootstersDbClient().SaveRipInfo(pid, ipAddress, DateTime.UtcNow);
                var bytes = await GetFileBytes(pid);
                response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new ByteArrayContent(bytes)};
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition =
                    ContentDispositionHeaderValue.Parse($"attachment; filename = \"product-{pid}.chkn\"");
                return response;

            } catch (FileNotFoundException) {
                response = new HttpResponseMessage(HttpStatusCode.NotFound) { 
                    Content = new StringContent("404 The file wasn't found on the server.")
                };
                return response;
            } catch (Exception exc) {
                var msg = $"500 Internal Server Error\n{exc.Message}\nStack Trace\n{exc.StackTrace}";
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(msg)
                };
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
                var urlTemplate = RipService.GetUrlTemplate(pid);
                //client.Timeout = new TimeSpan(0, 1, 0);

                var url = string.Format(urlTemplate, "_contents.json");
                var responseJson = await client.GetStringAsync(url);
                var json = $"{{productArray: {responseJson}}}";

                var list = JsonConvert.DeserializeObject<ProductContentList>(json);
                target = Path.Combine(target, pid.ToString());
                if (!Directory.Exists(target)) Directory.CreateDirectory(target);

                foreach (ProductContentItem item in list.productArray)
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
                        foreach (ProductContentItem item in list.productArray)
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

        

    }
}