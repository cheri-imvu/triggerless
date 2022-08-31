using log4net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Triggerless.Models;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class RadioController : BaseController
    {
        public static readonly ILog _log = LogManager.GetLogger(nameof(RadioController));
        private static BootstersDbClient _dbClient = new BootstersDbClient(_log);

        [Route("api/radio/list/{djname}/{count}"), HttpGet]
        public async Task<HttpResponseMessage> GetList(string djName, int count)
        {
            var response = await _dbClient.GetSongs(djName, count);
            return GetJsonResponseFromObject(response);
        }

        [Route("api/radio/post"), HttpPost]

        public async Task<HttpResponseMessage> Post([FromBody]TriggerlessRadioSong post)
        {
            var response = await _dbClient.PostSong(post);
            return GetJsonResponseFromObject(response);

        }

        // Important to note: IIS7 URLs cannot have a plus (+) symbol in the route
        // So don't escape the title, and scrub title of any plusses if they're there.

        [Route("api/radio/tune/{djName}/{title}"), HttpGet]
        public async Task<HttpResponseMessage> Tune(string djName, string title)
        {
            var response = await _dbClient.PostSong(new TriggerlessRadioSong { djName = djName, title = title });
            return GetJsonResponseFromObject(response);
        }
    }
}