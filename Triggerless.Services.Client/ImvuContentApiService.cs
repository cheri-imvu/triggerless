using System;
using System.Net.Http;
using Triggerless.Services.Common;


namespace Triggerless.Services.Client
{
    public class ImvuContentApiService : ApiService
    {
        public ImvuContentApiService()
        {
            _baseAddress = "https://userimages-akm.imvu.com/productdata/";
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler) { BaseAddress = new Uri(_baseAddress) };
        }

    }
}

