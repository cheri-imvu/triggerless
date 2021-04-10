using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class RipCountEntry
    {
        public string IpAddress { get; set; }
        public int Count { get; set; }

        public string URI { get; set; }
        public string URIWithProduct { get; set; }
    }

    public class RipEntry
    {
        public DateTime UtcDate { get; set; }
        public string IpAddress { get; set; }
        public long ProductId { get; set; }
        public long Id { get; set; }
    }

    public class RipEntryExt : RipEntry
    {
        public string ProductName { get; set; }
        public long CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string Rating { get; set; }
        public int RetailPrice { get; set; }
        public string ProductPage { get; set; }
        public string ProductImage { get; set; }
        public string ProductGender { get; set; }
        public bool IsVisible { get; set; }
        public bool IsPurchaseable { get; set; }
        public string Message { get; set; }

    }
}
