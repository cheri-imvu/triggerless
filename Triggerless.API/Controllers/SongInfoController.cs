using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Triggerless.Models;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class SongInfoController: BaseController
    {
        public static readonly ILog _log = LogManager.GetLogger(nameof(SongInfoController));

        [Route("api/SongInfo/{pid}"), HttpGet]
        public HttpResponseMessage SongInfo(int pid)
        {
            _log?.Debug($"GET api/SongInfo/{pid} start");
            try
            {
                var result = new NVorbisService(_log).GetSongInfo(pid);
                _log?.Debug($"GET api/SongInfo/{pid} {result.Entries.Count} song infos returned.");
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new JsonContent(result) };
            } catch (Exception exc)
            {
                _log?.Error($"SongInfo caused exception: {exc.Message}", exc);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) 
                    { Content = new JsonContent(new SongInfo 
                        { Success = false, ProductID = pid, Message = exc.Message })
                    };
            }
        }

        [Route("api/SongLength/{pid}/{location}"), HttpGet]
        public HttpResponseMessage SongLength(int pid, string location)
        {
            try
            {
                var result = new NVorbisService(_log).GetLength(pid, location);
                if (result > 0D) return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result.ToString()) };
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent($"Total time for '{location}' was unsuccessful") };
            }
            catch (Exception exc)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent($"{exc.Message}\n{exc.StackTrace}") };
            }
        }


    }
}