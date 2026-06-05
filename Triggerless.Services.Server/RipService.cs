//using static Triggerless.Services.Server.NVorbisService;

namespace Triggerless.Services.Server
{
    public class RipService
    {

        public RipService()
        {
        }
        public static string GetUrlTemplate(long pid) => $"https://userimages-akm.imvu.com/productdata/{pid}/1/{{0}}";

        public static string GetUrl(long pid, string filename) => string.Format(GetUrlTemplate(pid), filename);

    }
}
