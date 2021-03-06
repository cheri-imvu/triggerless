using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class LogConversation
    {
        public List<LogConversationEvent> Events { get; set; } = new List<LogConversationEvent>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class LogConversationEvent
    {
        public string Text { get; set; }
        public ImvuUser Author { get; set; } = new ImvuUser();
        public DateTime Time { get; set; }

    }
}
