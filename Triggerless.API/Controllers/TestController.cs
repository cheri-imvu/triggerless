using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Triggerless.API.Controllers
{
    public class TestController : ApiController {

        [Route("api/Test/{pid}")]

        public async Task<HttpResponseMessage> Get(int pid) {
            var payload = await GetString(pid);
            var response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(payload)
            };
            return response;

        }

        private async Task<string> GetString(int pid) {
            using (var client = new HttpClient()) {
                var urlTemplate = $"http://userimages-akm.imvu.com/productdata/{pid}/1/{{0}}";

                var url = string.Format(urlTemplate, "_contents.json");
                var responseJson = await client.GetStringAsync(url);
                var json = $"{{productArray: {responseJson}}}";
                return "Success";
            }
        }
    }
}
