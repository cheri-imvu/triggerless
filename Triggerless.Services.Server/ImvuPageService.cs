using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Services.Server
{
    public class ImvuPageService: ImvuApiService
    {
        public ImvuPageService()
        {
            _baseAddress = "https://www.imvu.com";
            var cookies = new CookieContainer();
            _handler = new HttpClientHandler { CookieContainer = cookies };
            cookies.Add(new Cookie("osCsid", OsCsid, "/", ".imvu.com"));
            _client = new HttpClient(_handler) { BaseAddress = new Uri(_baseAddress) };

        }

    }
}
