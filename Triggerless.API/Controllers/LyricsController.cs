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
                    response.Content = new StringContent(result.Lyrics,
                        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), "application/json");
                    response.Content.Headers.ContentDisposition = cd;
                    break;
                default: throw new Exception($"What the hell happened?\n{result.Lyrics}");
            }
            return response;
        }

        [Route("{productId:long}"), HttpPost]        // GET: Lyrics
        public async Task<HttpResponseMessage> Post(long productId)
        {
            var response = new HttpResponseMessage();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"ProductID: {productId}");
            sb.AppendLine($"Request is null: {Request is null}");
            sb.AppendLine($"Content is null: {Request.Content is null}");

            try
            {
                var lyrics = await Request.Content.ReadAsStringAsync(); // raw body
                sb.AppendLine($"Lyrics not null: {lyrics == null}");
                
                TriggerbotLyricsEntry.EntryStatus result = TriggerbotLyricsEntry.EntryStatus.Empty;

                if (string.IsNullOrWhiteSpace(lyrics))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Content = new StringContent("Dude, you need to send the lyrics in the HTTP body");
                }
                else
                {
                    sb.AppendLine($"Opening Database");
                    var dbClient = new BootstersDbClient();
                    try
                    {
                        sb.AppendLine("About to save lyrics");
                        result = await dbClient.SaveLyrics(productId, lyrics);
                        sb.AppendLine($"SaveLyrics gave: {response}");
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
            }
            catch (Exception exc)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent(sb.ToString() + exc.ToString());
            }

            return response;

        }
    }
}