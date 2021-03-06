using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Triggerless.Services.Common;

namespace Triggerless.Services.Server
{
    public class ImvuApiService: ApiService {

        public ImvuApiService() {
            _baseAddress = "https://api.imvu.com";
            var cookies = new CookieContainer();
            var osCsid = Settings.Default.osCsid;
            _handler = new HttpClientHandler {CookieContainer = cookies};
            cookies.Add(new Cookie("osCsid", osCsid, "/", ".imvu.com"));
            _client = new HttpClient(_handler) {BaseAddress = new Uri(_baseAddress)};

        }
    }
}