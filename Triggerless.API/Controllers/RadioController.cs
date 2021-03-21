using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Triggerless.Models;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{

    public class RadioController : BaseController
    {
        [Route("api/radio/hours/{hours}"), HttpGet]
        public async Task<HttpResponseMessage> Get(double hours)
        {
            var response = await BootstersDbService.GetSongs(hours);
            return GetResponseFromObject(response);
        }

        [Route("api/radio/post")]

        public async Task<HttpResponseMessage> Post(TriggerlessRadioSong post)
        {
            var response = await BootstersDbService.PostSong(post);
            return GetResponseFromObject(response);

        }
    }
}