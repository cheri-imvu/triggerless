using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Triggerless.Services.Server;

namespace Triggerless.API.Models
{
    public class Discord : IDisposable
    {
        private static DiscordClient _client;
        private static ulong PublicChannelId => ulong.Parse(ConfigurationManager.AppSettings["DiscordChannelPublic"]);
        private static ulong PrivateChannelId => ulong.Parse(ConfigurationManager.AppSettings["DiscordChannelPrivate"]);

        public static async Task<int> CleanupChannel()
        {
            if (_client == null)
            {
                await GetClient().ConfigureAwait(false);
            }
            DiscordChannel channel = await _client.GetChannelAsync(id: PublicChannelId);
            var messages = await channel.GetMessagesAsync(1000).ConfigureAwait(false);
            foreach (var message in messages)
            {
                if (message.Embeds.Count > 0)
                {
                    var embed = message.Embeds[0];
                    if (embed.Title == "Scan Failure")
                    {
                        await message.DeleteAsync("bug message").ConfigureAwait(false);
                    }
                }

            }
            return 0;
        }

        public static async Task<string> GetInviteLink()
        {
            string result = string.Empty;
            using (var client = new HttpClient())
            {
                var code = await client.GetStringAsync("https://triggerless.com/triggerbot/invite-code.txt").ConfigureAwait(false);
                result = $"https://discord.gg/{code}";
            }
            return result;
        }


        public static async Task<int> SendMessage(string title, string body, ulong cid = 0)
        {
            try
            {
                if (_client == null)
                {
                    await GetClient().ConfigureAwait(false);
                }

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Chartreuse,
                    Title = title,
                    Description = body
                };

                DiscordChannel channel = await _client.GetChannelAsync(PublicChannelId);

                await channel.SendMessageAsync(embed).ConfigureAwait(false);

                BootstersDbClient client = new BootstersDbClient();
                //client.SaveEvent()
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        private static async Task GetClient()
        {
            string token;
            token = ConfigurationManager.AppSettings["DiscordBotToken"];

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            _client = new DiscordClient(discordConfig);
            _client.Ready += (sender, args) => Task.CompletedTask;

            await _client.ConnectAsync().ConfigureAwait(false);
        }

        public static void ShutdownClient()
        {
            _client.DisconnectAsync().RunSynchronously();
            _client.Dispose();
            _client = null;
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.DisconnectAsync().RunSynchronously();
                _client.Dispose();
                _client = null;
            }
        }

        public class IpInfo
        {
            public string cityName { get; set; }
            public string regionName { get; set; }
            public string countryName { get; set; }
        }
    }
}
