using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Triggerless.Services.Server;
using log4net;

namespace Triggerless.API.Controllers
{
    public class AvatarCardController : BaseController {

        public static readonly ILog _log = LogManager.GetLogger(nameof(AvatarCardController));

        [Route("api/avatarcard/{idOrName}")]
        public async Task<HttpResponseMessage> Get(string idOrName) {
            _log?.Debug($"Get api/avatarcard/{idOrName}");
            using (var client = new ImvuApiClient(_log))
            {
                var json = await client.GetAvatarCardJson(idOrName);
                _log?.Debug($"Success api/avatarcard/{idOrName}");
                return GetJsonResponse(json);
            }
            
        }


    }
}