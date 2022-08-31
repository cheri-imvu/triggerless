using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class SongInfo
    {
        public bool Success { get; set; }
        public int ProductID { get; set; }
        public string Message { get; set; }
        public List<SongEntry> Entries { get; set; } = new List<SongEntry>();

    }

    public class SongEntry
    {
        public string Location { get; set; }
        public string Filename { get; set; }
        public string Trigger { get; set; }
        public double Sequence { get; set; }
        public double Length { get; set; }
        public string TriggerPrefix { get; set; }
    }
}
