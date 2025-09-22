using System;
using System.Net;
using System.Net.Http;

namespace Triggerless.Services.Server
{
    public class ImvuPageService: ImvuApiService
    {
        public ImvuPageService()
        {
            _baseAddress = "https://www.imvu.com";
            var cookies = new CookieContainer();
            cookies.Add(new Cookie("osCsid", OsCsid, "/", ".imvu.com"));
            _handler = new HttpClientHandler { CookieContainer = cookies };
            _client = new HttpClient(_handler) { BaseAddress = new Uri(_baseAddress) };

        }

    }
}
