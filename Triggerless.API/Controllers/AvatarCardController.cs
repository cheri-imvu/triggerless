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

namespace Triggerless.API.Controllers
{
    public class AvatarCardController : BaseController {

        // GET api/<controller>/5
        [Route("api/avatarcard/{idOrName}")]
        public async Task<HttpResponseMessage> Get(string idOrName) {

            using (var client = new ImvuApiClient())
            {
                var json = await client.GetAvatarCardJson(idOrName);
                return GetJsonResponse(json);
            }
            
        }


    }
}