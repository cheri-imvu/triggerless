using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Triggerless.API.Controllers
{
    public class PingController : ApiController
    {
        [HttpGet]
        [Route("api/ping")]
        public IHttpActionResult Ping()
        {
            try
            {
                return Ok("pong");
            }
            catch (Exception ex)
            {
                return Ok(ex.ToString());
            }
        }
    }
}
