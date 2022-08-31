using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using Triggerless.Models;
using Triggerless.XAFLib;
//using static Triggerless.Services.Server.NVorbisService;
using log4net;

namespace Triggerless.Services.Server
{
    public class RipService
    {
        private ILog _log;

        public RipService(ILog log = null)
        {
            _log = log;
        }
        public static string GetUrlTemplate(int pid) => $"https://userimages-akm.imvu.com/productdata/{pid}/1/{{0}}";

        public static string GetUrl(int pid, string filename) => string.Format(GetUrlTemplate(pid), filename);

    }
}
