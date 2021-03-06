using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Triggerless.Models;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class UserController : BaseController
    {
        // GET api/<controller>/5
        [Route("api/user/{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {
            var user = await GetUser(id);
            return GetResponseFromObject(user);
        }

        public async Task<ImvuUser> GetUser(long id)
        {
            var client = new ImvuApiClient();
            return await client.GetUser(id);

            // GET: User
        }

        [Route("api/username/{userName}")]
        public async Task<HttpResponseMessage> GetByName(string userName)
        {
            var client = new ImvuApiClient();
            var user = await client.GetUserByName(userName);
            return GetResponseFromObject(user);
        }
    }
}