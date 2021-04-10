using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class RipLogController : BaseController
    {
        [HttpGet, Route("api/riplog/summary")]
        public async Task<HttpResponseMessage> RipLogSummary()
        {
            return GetJsonResponseFromObject(await BootstersDbClient.RipLogSummary());
        }

        [HttpGet, Route("api/riplog/ip/{a1}.{a2}.{a3}.{a4}/")]
        public async Task<HttpResponseMessage> RipLogByIp(int a1, int a2, int a3, int a4)
        {
            var ip = $"{a1}.{a2}.{a3}.{a4}";
            return GetJsonResponseFromObject(await BootstersDbClient.RipLogEntriesByIp(ip));
        }

        [HttpGet, Route("api/riplog/ipx/{a1}.{a2}.{a3}.{a4}/")]
        public async Task<HttpResponseMessage> RipLogByIpx(int a1, int a2, int a3, int a4)
        {
            var ip = $"{a1}.{a2}.{a3}.{a4}";
            return GetJsonResponseFromObject(await BootstersDbClient.RipLogEntriesByIpExt(ip));
        }

        [HttpGet, Route("api/riplog/test/{entry}")]
        public async Task<HttpResponseMessage> Test (string entry)
        {
            return GetJsonResponseFromObject(new { Message = $"You entered {entry}" });
        }
    }
}
