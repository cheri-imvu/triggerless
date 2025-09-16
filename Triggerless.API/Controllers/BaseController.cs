using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using Triggerless.API.Models;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Triggerless.API.Controllers
{
    public class BaseController: ApiController
    {
        protected HttpResponseMessage GetJsonResponse(string json)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            // we take care of this at web.config now.
            // response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        protected HttpResponseMessage GetJsonResponseFromObject(object thing)
        {
            return GetJsonResponse(JsonConvert.SerializeObject(thing));
        }

        protected HttpResponseMessage GetBinaryResponse(byte[] bytes, string filename, string mediaType = "application/octet-stream")
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new ByteArrayContent(bytes);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            response.Content.Headers.ContentDisposition =
                ContentDispositionHeaderValue.Parse($"attachment; filename = \"{filename}\"");
            return response;
        }

        protected string GetClientIp()
        {
            string ipAddress = null;

            // Try from MS_HttpContext (IIS hosted)
            if (Request.Properties.ContainsKey("MS_HttpContext"))
            {
                ipAddress = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            // Fallback to OWIN property
            if (string.IsNullOrWhiteSpace(ipAddress) && Request.Properties.ContainsKey("RemoteEndpointMessageProperty"))
            {
                dynamic remoteEndpoint = Request.Properties["RemoteEndpointMessageProperty"];
                ipAddress = remoteEndpoint?.Address;
            }

            // Final fallback
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = HttpContext.Current?.Request?.UserHostAddress;
            }

            // Normalize ::1 to 127.0.0.1 for localhost
            if (ipAddress == "::1")
                ipAddress = "127.0.0.1";

            // Convert other IPv6 addresses to IPv4 if possible
            if (System.Net.IPAddress.TryParse(ipAddress, out var ip))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    ip = System.Net.Dns.GetHostEntry(ip).AddressList
                        .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if (ip != null)
                    {
                        ipAddress = ip.ToString();
                    }
                }
            }

            return ipAddress ?? Discord.UNKNOWN_IP;
        }

    }
}