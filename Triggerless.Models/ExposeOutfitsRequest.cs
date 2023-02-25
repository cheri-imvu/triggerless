using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class ExposeOutfitsRequest
    {
        public ExposeOutfitsRequestEntry[] Entries { set; get; }
    }

    public class ExposeOutfitsRequestEntry
    {
        public long AvatarId { get; set; }
        public long[] ProductIds { get; set; }
    }

    public class ExposeOutfitsResponse
    {
        public ExposeOutfitsResponseEntry[] Entries { set; get; }

    }

    public class ExposeOutfitsResponseEntry
    {
        public ImvuUser User { get; set; }
        public ImvuProduct[] Products { get; set; }

    }
}
