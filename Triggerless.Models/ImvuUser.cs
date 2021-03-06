using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class ImvuUser: ImvuApiResult
    {
        [JsonProperty(PropertyName = "id")]
        public string ApiId { get; set; }

        [JsonProperty(PropertyName = "avatarname")]
        public string AvatarName { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        [JsonProperty(PropertyName = "member_since")]
        public DateTime? MemberSince { get; set; }

        [JsonProperty(PropertyName = "location")]
        public ImvuUserLocation Location { get; set; } = new ImvuUserLocation();

        [JsonProperty(PropertyName = "interests")]
        public string[] Interests { get; set; }

        [JsonProperty(PropertyName = "photo")]
        public string Photo { get; set; }

        [JsonProperty(PropertyName = "is_vip")]
        public bool? IsVIP { get; set; }

        [JsonProperty(PropertyName = "is_ap")]
        public bool? IsAP { get; set; }

        [JsonProperty(PropertyName = "sex")]
        public string Sex { get; set; }

        [JsonProperty(PropertyName = "customer_id")]
        public long Id { get; set; }


    }

    public class ImvuUserLocation
    {
        [JsonProperty(PropertyName = "location_code")]
        public string LocationCode { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }
    }
}
