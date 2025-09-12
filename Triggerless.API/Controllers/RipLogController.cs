using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Triggerless.Services.Server;
using System.Linq;
using log4net;

namespace Triggerless.API.Controllers
{
    public class RipLogController : BaseController
    {
        public static readonly ILog _log = LogManager.GetLogger(nameof(RipLogController));
        private static BootstersDbClient _dbClient = new BootstersDbClient(_log);

        [HttpGet, Route("api/riplog/summary")]
        public async Task<HttpResponseMessage> RipLogSummary()
        {
            return GetJsonResponseFromObject(await _dbClient.RipLogSummary());
        }

        [HttpGet, Route("api/riplog/ip/{a1}.{a2}.{a3}.{a4}")]
        public async Task<HttpResponseMessage> RipLogByIp(int a1, int a2, int a3, int a4)
        {
            var ip = $"{a1}.{a2}.{a3}.{a4}";
            return GetJsonResponseFromObject(await _dbClient.RipLogEntriesByIp(ip));
        }

        [HttpGet, Route("api/riplog/ipx/{a1}.{a2}.{a3}.{a4}")]
        public async Task<HttpResponseMessage> RipLogByIpx(int a1, int a2, int a3, int a4)
        {
            var ip = $"{a1}.{a2}.{a3}.{a4}";
            return GetJsonResponseFromObject(await _dbClient.RipLogEntriesByIpExt(ip));
        }

        [HttpGet, Route("api/riplog/date/{dateString}/{hours}"), Route("api/riplog/date/{dateString}")]
        public async Task<HttpResponseMessage> RipLogByDate (string dateString, int? hours = 24)
        {
            var entries = await _dbClient.RipLogEntriesByUtcDate(dateString, hours.Value);
            if (entries == null || !entries.Any()) return GetJsonResponseFromObject(new { Message = $"Invalid date string or no data available" });
            return GetJsonResponseFromObject(entries);
        }
    }
}
