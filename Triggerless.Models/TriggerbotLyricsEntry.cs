using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class TriggerbotLyricsEntry
    {
        public long Id { get; set; }
        public string Lyrics { get; set; }
        public DateTime Modified { get; set; }
        public int Version { get; set; }    
    }
}
