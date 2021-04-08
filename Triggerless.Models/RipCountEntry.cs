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
    }

    public class RipEntry
    {
        public DateTime Date { get; set; }
        public string IpAddress { get; set; }
        public long ProductId { get; set; }
        public long Id { get; set; }
    }
}
