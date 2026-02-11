using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Triggerless.Models
{
    public class TriggerResult
    {
        public int Sequence { get; set; }
        public string TriggerName { get; set; }
        public string OggName { get; set; }
        public double LengthMS { get; set; }
    }

    public class TriggerEntry
    {
        public long ProductId { get; set; }
        public string TriggerName { get; set; }
        public string OggName { get; set; }
        public string Location { get; set; }
        public int Sequence { get; set; }
        public string Prefix { get; set; }
        public double LengthMS { get; set; }
        public double WaitMS { get; set; }
    }

    public class ProductRecord
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string CreatorName { get; set; }
        public string ImageLocation { get; set; }
        public byte[] ImageBytes { get; set; }
        public DateTime DateCreated { get; set; }
        public long AddedBy { get; set; }
    }

    public class CollectorPayload: ProductRecord
    {
        public List<TriggerEntry> Triggers { get; set; } = new List<TriggerEntry>();
    }

    public class CollectorResponsePayload : CollectorPayload 
    {
        public ScanResultType Result { get; set; }
        public string Message { get; set; }
    }

    public class ScanResult
    {
        public long ProductId { get; set; }
        public ScanResultType Result { get; set; }
        public string Message { get; set; }
        public List<TriggerResult> TriggerResults { get; set; }
    }

    public enum ScanResultType
    {
        Success = 0,
        DatabaseError = 1,
        NetworkError = 2,
        DecodingError = 3,
        SystemError = 4,
        Pending = 5,
        JsonError = 6,
        NoUsefulTriggers = 7,
        XmlError = 8,
        ZeroTriggers = 9,
        NullPrefixes = 10,
        MixedPrefixes = 11,
        NotEnoughOggs = 12,
        ProductUnavailable = 13
    }

    public class ContentsJsonItem
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("original_dimensions")] public string OriginalDimensions { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("tags")] public string[] Tags { get; set; }
        public string Location => string.IsNullOrWhiteSpace(Url) ? Name : Url;
    }

    public class ProductPayload
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string CreatorName { get; set; }
        public byte[] ImageBytes { get; set; }
        public bool HasOgg { get; set; }
    }
}
