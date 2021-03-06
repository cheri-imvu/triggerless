using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Triggerless.Models;

namespace Triggerless.Services.Client
{
    public class FileReader
    {
        public LogConversation ReadFile(string fileName)
        {

            using (var reader = new StreamReader(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                string line;
                string[] bigSplitter = new[] { "INFO: " };
                string[] smallSplitter = new[] { "/u" };
                var rxAuthorId = new Regex(@"^notifyNewMessage called for (?<authid>\d+)$");
                var rxUnixTime = new Regex(@"/(?<unixtime>[0123456789.]+)$");
                var zeroDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                var rxText1 = new Regex("['](?<text>.*)[']");
                var rxText2 = new Regex("[\"](?<text>.*)[\"]"); // color confused, sorry

                var events = new List<LogConversationEvent>();

                while ((line = reader.ReadLine()) != null)
                {
                    var bigPieces = line.Split(bigSplitter, StringSplitOptions.None);
                    if (bigPieces.Length != 2) continue;
                    if (!bigPieces[1].StartsWith("notifyNewMessage")) continue;

                    var smallPieces = bigPieces[1].Split(smallSplitter, StringSplitOptions.None);
                    if (smallPieces.Length != 2) continue;

                    var match = rxAuthorId.Match(smallPieces[0]);
                    if (!match.Success) continue;

                    // Author ID

                    long authorId;
                    var authorIdText = match.Groups["authid"].Value;
                    if (!long.TryParse(authorIdText, out authorId)) continue;

                    var @event = new LogConversationEvent();
                    @event.Author.Id = authorId;

                    // Time

                    match = rxUnixTime.Match(smallPieces[1]);
                    if (match.Success)
                    {
                        var cap = match.Groups["unixtime"].Value;
                        double unixTime;
                        if (double.TryParse(cap, out unixTime))
                        {
                            @event.Time = zeroDate.AddSeconds(unixTime);
                        }
                    }

                    // Text

                    var rxText = smallPieces[1].StartsWith("'") ? rxText1 : rxText2;
                    match = rxText.Match(smallPieces[1]);
                    if (match.Success)
                    {
                        var text = match.Groups["text"].Value;
                        if (!text.StartsWith("*"))
                        {
                            @event.Text = text;
                        }
                    }

                    if (@event.Text != null) events.Add(@event);

                }

                return new LogConversation
                {
                    Events = events
                };
            }
        }
    }
}
