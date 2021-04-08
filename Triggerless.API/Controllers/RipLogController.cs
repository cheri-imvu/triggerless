using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            return GetResponseFromObject(await BootstersDbService.IpAddresses());
        }

        [HttpGet, Route("api/riplog/ip/{a1}.{a2}.{a3}.{a4}")]
        public async Task<HttpResponseMessage> RipLogByIp(int a1, int a2, int a3, int a4)
        {
            var ip = $"{a1}.{a2}.{a3}.{a4}";
            return GetResponseFromObject(await BootstersDbService.LogEntriesByIp(ip));
        }
    }
}
