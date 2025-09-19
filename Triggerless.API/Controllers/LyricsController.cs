using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    [RoutePrefix("api/lyrics")]
    public class LyricsController : BaseController
    {

        [Route("{productId:long}"), HttpGet]        // GET: Lyrics
        public async Task<HttpResponseMessage> Get(long productId)
        {
            var dbClient = new BootstersDbClient();
            var result = await dbClient.GetLyrics(productId);
            var response = new HttpResponseMessage();

            switch (result.Status)
            {
                case Triggerless.Models.TriggerbotLyricsEntry.EntryStatus.Error:
                    response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    response.Content = new StringContent($"{result.Lyrics}");
                    break;
                case Triggerless.Models.TriggerbotLyricsEntry.EntryStatus.NotFound:
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    response.Content = new StringContent($"The lyrics for ProductId {productId} have not been added to the database yet. ");
                    break;
                case Triggerless.Models.TriggerbotLyricsEntry.EntryStatus.Success:
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    var cd = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"{productId}.lyrics"
                    };
                    response.Content.Headers.ContentDisposition = cd;
                    response.Content = new StringContent(result.Lyrics,
                        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), "application/json");
                    break;
                default: throw new Exception($"What the hell happened?\n{result.Lyrics}");
            }
            return response;
        }

        [Route("{productId:long}"), HttpPost]        // GET: Lyrics
        public async Task<HttpResponseMessage> Post(long productId, [FromBody] string lyrics)
        {
            var response = new HttpResponseMessage();
            var dbClient = new BootstersDbClient();
            var result = await dbClient.SaveLyrics(productId, lyrics);

            // save stuff
            return response;

        }
    }
}