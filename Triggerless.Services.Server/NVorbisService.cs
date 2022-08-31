using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using log4net;
using NVorbis;

namespace Triggerless.Services.Server
{
    public class NVorbisService
    {
        private ILog _log;

        public NVorbisService(ILog log = null)
        {
            _log = log;
        }

        public class GetLengthsRequest
        {
            public int PID { get; set;  }
            public IEnumerable<string> Locations { get; set; }
        }

        public class GetLengthsResponse
        {
            public int PID { get; set; }
            public IEnumerable<(string, double)> Results { get; set; }
        }

        public double GetLength(int pid, string location)
        {
            var url = RipService.GetUrl(pid, location);
            var result = -1D;
            using (var client = new HttpClient())
            {
                var success = false;
                var tries = 0;
                var maxTries = 5;
                Exception lastException = null;
                for (int i = 0; i < maxTries; i++)
                {
                    byte[] bytes = client.GetByteArrayAsync(url).Result;
                    using (var stream = new MemoryStream(bytes))
                    {
                        try
                        {
                            var v = new VorbisReader(stream);
                            result = v.TotalTime.TotalMilliseconds;
                            success = true;
                            break;

                        }
                        catch (Exception exc)
                        {
                            lastException = exc;
                            tries++;
                            Thread.Sleep(100);
                        }
                    }
                }
                if (success) return result;
                throw new ArgumentException($"Unable to get TotalTime for '{location}'", lastException);                
            }
        }

        public GetLengthsResponse GetLengths(GetLengthsRequest request)
        {
            _log?.Debug($"{nameof(NVorbisService)}.{nameof(GetLengths)}");
            GetLengthsResponse result = new GetLengthsResponse();
            result.PID = request.PID;
            List<(string, double)> list = new List<(string, double)>();
            MemoryStream stream;
            int reqLocCount = 0;

            using (var client = new HttpClient())
            {
                foreach (string location in request.Locations)
                {
                    reqLocCount++;
                    var url = RipService.GetUrl(request.PID, location);
                    byte[] bytes;
                    try { 
                        bytes = client.GetByteArrayAsync(url).Result; 
                    }
                    catch (Exception httpExc)
                    {
                        _log?.Warn($"Failed to retrieve {url}", httpExc);
                        continue;
                    }
                    
                    using (stream = new MemoryStream(bytes))
                    {
                        try
                        {
                            var v = new VorbisReader(stream);
                            list.Add((location, v.TotalTime.TotalMilliseconds));
                            _log?.Debug($"\t{location} - {v.TotalTime.TotalMilliseconds}");
                        }
                        catch (Exception exc)
                        {
                            _log?.Warn($"{location}: unable to deduce TotalTime", exc);
                            //throw new ArgumentException($"Unable to get TotalTime from {url}", exc);
                        }

                    }
                }
            }

            if (list.Count != reqLocCount)
            {
                _log?.Error($"This result has errors. {reqLocCount} ogg files requested, {list.Count} successful");
            }
            result.Results = list;
            return result;
        }
    }
}
