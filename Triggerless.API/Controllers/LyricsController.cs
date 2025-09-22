using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Triggerless.Services.Server;
using Triggerless.Models;

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
                case TriggerbotLyricsEntry.EntryStatus.Error:
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Content = new StringContent($"{result.Lyrics}");
                    break;
                case TriggerbotLyricsEntry.EntryStatus.NotFound:
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Content = new StringContent($"The lyrics for ProductId {productId} have not been added to the database yet. ");
                    break;
                case TriggerbotLyricsEntry.EntryStatus.Success:
                    response.StatusCode = HttpStatusCode.OK;
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
        public async Task<HttpResponseMessage> Post(long productId)
        {
            var response = new HttpResponseMessage();
            var lyrics = await Request.Content.ReadAsStringAsync(); // raw body
            TriggerbotLyricsEntry.EntryStatus result = TriggerbotLyricsEntry.EntryStatus.Empty;

            if (string.IsNullOrWhiteSpace(lyrics))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent("Dude, you need to send the lyrics in the HTTP body");
            }
            else
            {
                var dbClient = new BootstersDbClient();
                try
                {
                    result = await dbClient.SaveLyrics(productId, lyrics);
                    if (result == TriggerbotLyricsEntry.EntryStatus.Error)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                        response.Content = new StringContent("Unable to save lyrics to database");
                    }
                    else
                    {
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StringContent("Lyrics save was successful");
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Content = new StringContent(ex.ToString());
                }

            }

            return response;

        }
    }
}