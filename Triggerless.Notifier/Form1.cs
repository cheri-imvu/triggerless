using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Notifications;
using Triggerless.Models;
using Triggerless.Services.Server;

namespace Triggerless.Notifier
{
    public partial class Form1 : Form
    {
        private ConvoResponse _lastResponse;
        public Form1()
        {
            InitializeComponent();
        }

        private void SendToast(string firstLine, string secondLine)
        {
            var xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var text = xml.GetElementsByTagName("text");
            text[0].AppendChild(xml.CreateTextNode(firstLine));
            text[1].AppendChild(xml.CreateTextNode(secondLine));

            var toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier("IMVU-Notifier").Show(toast);
        }

        private async Task GetLastResponse()
        {
            ConvoResponse convoResponse = null;
            using (var client = new ImvuApiClient())
            {
                convoResponse = await client.ConversationResponse();
            }
            

            if (_lastResponse == null)
            {
                _lastResponse = convoResponse;
                return;
            }

            var latestDate = _lastResponse.Conversations.Select(c => c.LastMessage.Created).Max();
            var laterMessages = convoResponse.Conversations.Where(c => c.LastMessage.Created > latestDate);

            foreach (var convo in laterMessages)
            {
                var sender = convo.Participants.Where(p => p.Name != "Cheri").First().Name;
                if (sender.Contains("_")) continue;
                var message = convo.LastMessage;

                var firstLine = $"{sender} sent an IMVU Message";
                var secondLine = message.Payloads.Where(mp => mp.Content != null).First().Content;

                SendToast(firstLine, secondLine);
            }
            _lastResponse = convoResponse;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetLastResponse().Wait();
        }
    }
}
