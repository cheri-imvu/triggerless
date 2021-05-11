using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Triggerless.Models
{
    public class MessagePayload
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public string GiftType { get; set; }
        public string GiftWrap { get; set; }
        public string GiftId { get; set; }
    }

    public class Message
    {
        public List<MessagePayload> Payloads { get; set; } = new List<MessagePayload>();
        public string SentBy { get; set; }
        public DateTime Created { get; set; }
        public string Sender { get; set; }
        public string MessageId { get; set; }
        public string OriginAssignedId { get; set; }
        public string BourbonSender { get; set; }
    }

    public class Participant
    {
        public string Profile { get; set; }
        public string User { get; set; }
        public string Name { get; set; }
    }

    public class Conversation
    {
        public long Id { get; set; }
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public Message LastMessage { get; set; }
        public int UnreadMessages { get; set; }
        public string Label { get; set; }

    }

    public class ConvoResponse
    {
        public List<Conversation> Conversations { get; set; } = new List<Conversation>();

        public List<Message> Messages { get; set; } = new List<Message>();

        public static ConvoResponse FromJson(string json)
        {
            var result = new ConvoResponse();
            var convos = new List<Conversation>();

            Regex rx = new Regex(@"https://api.imvu.com/conversation/conversation-(\d+)$");
            JToken root = JToken.Parse(json);
            foreach (JProperty entry in root["denormalized"])
            {
                var m = rx.Match(entry.Name);
                if (!m.Success) continue;

                var data = entry.First["data"];
                var convo = new Conversation();
                convo.Id = long.Parse(m.Groups[1].Value);
                convo.Label = data["label"].Value<string>();
                convo.UnreadMessages = data["unread_messages"].Value<int>();
                convo.Participants = data["participants"].Select(p => new Participant
                {
                    Profile = p["profile"].Value<string>(),
                    User = p["user"].Value<string>(),
                    Name = p["name"].Value<string>()
                }).ToList();

                var lm = data["last_message"];
                convo.LastMessage = new Message
                {
                    BourbonSender = lm["bourbon_sender"].Value<string>(),
                    Created = lm["created"].Value<DateTime>(),
                    SentBy = lm["sent_by"].Value<string>(),
                    Sender = lm["sender"].Value<string>(),
                    MessageId = lm["message_id"].Value<string>(),
                    OriginAssignedId = lm["origin_assigned_id"].Value<string>(),
                    Payloads = lm["payloads"].Select(pl => new MessagePayload
                    {
                        Content = pl["content"] == null ? null : pl["content"].Value<string>(),
                        Type = pl["type"] == null ? null : pl["type"].Value<string>(),
                        GiftId = pl["giftId"] == null ? null : pl["giftId"].Value<string>(),
                        GiftType = pl["giftType"] == null ? null : pl["giftType"].Value<string>(),
                        GiftWrap = pl["giftWrap"] == null ? null : pl["giftWrap"].Value<string>()

                    }).ToList()
                };
            }

            return result;
        }
    }
}
