using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Triggerless.Services.Common
{
    public abstract class ApiService: IDisposable {
        protected HttpClientHandler _handler;
        protected HttpClient _client;
        protected string _baseAddress;

        public ApiService() {

        }

        private void PrepareForJson()
        {
            // ugly :(
            if (_client.DefaultRequestHeaders.Accept.Count == 0)
                _client?.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<byte[]> GetBytes(string relativeUri) {
            var result = await _client?.GetByteArrayAsync(relativeUri);
            return result;
        }

        public async Task<string[]> GetLines(string relativeUri)
        {
            var text = await _client?.GetStringAsync(relativeUri);
            return text.Split('\n');
        }

        public async Task<string> GetJsonString(string relativeUri) {
            PrepareForJson();
            return await _client?.GetStringAsync(relativeUri);
        }

        public async Task<string> GetString(string relativeUri)
        {
            return await _client?.GetStringAsync(relativeUri);
        }

        public async Task<Stream> GetStream(string relativeUrl) {
            return await _client?.GetStreamAsync(relativeUrl);
        }

        public async Task<JObject> GetJObject(string relativeUrl) {
            var response = await GetJsonString(relativeUrl);
            var result = JObject.Parse(response);
            return result;
        }

        public async Task<T> GetPoco<T>(string relativeUrl) {
            string json = await GetJsonString(relativeUrl);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<TResp> PostPoco<TReq, TResp>(string relativeUri, TReq poco) {
            string jsonOut = JsonConvert.SerializeObject(poco);
            var response = await _client.PostAsync(relativeUri, new StringContent(jsonOut));
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResp>(json);
        }
        
        public void Dispose() {
            _client?.Dispose();
            _handler?.Dispose();
        }
    }
}