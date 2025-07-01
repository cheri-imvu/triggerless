using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Triggerless.API.Models
{
    public class Discord : IDisposable
    {
        public const string UNKNOWN_IP = "unknown";
        private static DiscordClient _client;
        private static ulong _channelId = 1360224553208516638;

        public static async Task<int> CleanupChannel()
        {
            if (_client == null)
            {
                await GetClient().ConfigureAwait(false);
            }
            DiscordChannel channel = await _client.GetChannelAsync(_channelId);
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
        public static async Task<int> SendMessage(string title, string body)
        {
            try
            {
                if (_client == null)
                {
                    await GetClient().ConfigureAwait(false);
                }

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.HotPink,
                    Title = title,
                    Description = body
                };

                DiscordChannel channel = await _client.GetChannelAsync(_channelId);

                await channel.SendMessageAsync(embed).ConfigureAwait(false);

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

        public static async Task<string> GetLocationFromIpAsync(string ipAddress)
        {
            string url = $"https://free.freeipapi.com/api/json/{ipAddress}";

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var ipInfo = JsonConvert.DeserializeObject<IpInfo>(json);

                    if (ipInfo == null || string.IsNullOrWhiteSpace(ipInfo.cityName))
                        return "Unknown location";

                    return $"{ipInfo.cityName}, {ipInfo.regionName}, {ipInfo.countryName}";
                }
                catch (Exception ex)
                {
                    // You can log this if needed
                    return $"Error: {ex.Message}";
                }
            }
        }

    }
}
