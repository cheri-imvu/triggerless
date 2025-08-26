using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    [RoutePrefix("api/lyrics")]
    public class LyricsController : BaseController
    {

        [Route("/")]
        // GET: Lyrics
        public async Task<HttpResponseMessage> Get(long productId)
        {

        }
    }
}