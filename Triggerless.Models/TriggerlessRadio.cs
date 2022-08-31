using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class TriggerlessRadioSongs
    {
        public string[] titles { get; set; }
        public string status { get; set; }
    }

    public class TriggerlessRadioSong
    {
        public string djName { get; set; }
        public string title { get; set; }
    }

    public class TriggerlessPostResponse
    {
        public string status { get; set; }

    }
}
