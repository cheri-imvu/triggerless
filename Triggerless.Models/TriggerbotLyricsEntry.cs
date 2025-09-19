using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{

    public class TriggerbotLyricsEntry
    {
        public enum EntryStatus
        {
            Empty = -1,
            Success = 0,
            NotFound = 1,
            Error = 2,
        }

        public long Id { get; set; }
        public string Lyrics { get; set; }
        public DateTime Modified { get; set; }
        public int Version { get; set; }
        public EntryStatus Status { get; set; } = EntryStatus.Empty;
    }
}
