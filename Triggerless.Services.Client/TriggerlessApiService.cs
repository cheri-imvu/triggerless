using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Services.Common;

namespace Triggerless.Services.Client
{
    public class TriggerlessApiService: ApiService
    {
        public TriggerlessApiService()
        {
            _baseAddress = "http://localhost:61120/api/";
            //_baseAddress = "https://triggerless.com/api/";
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler) { BaseAddress = new Uri(_baseAddress) };
        }

}
}
