using Newtonsoft.Json;

namespace Triggerless.Models
{
    public class ImvuApiResult
    {

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
