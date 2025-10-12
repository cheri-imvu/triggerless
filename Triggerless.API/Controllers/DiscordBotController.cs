using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Triggerless.API.Models;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class DiscordMessageRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }


    [RoutePrefix("api/bot")]
    public class DiscordBotController : CustomerController
    {

        [HttpPost]
        [Route("sendmessage")]
        public async Task<IHttpActionResult> SendMessage([FromBody] DiscordMessageRequest request)
        {
            try
            {
                // Check API key
                var expectedKey = ConfigurationManager.AppSettings["DiscordBotToken"];

                // Validate payload
                if (string.IsNullOrWhiteSpace(request?.Title) || string.IsNullOrWhiteSpace(request?.Body))
                {
                    return BadRequest("Title and body are required.");
                }

                var body = request.Body;
                var location = "Internal Network";
                var ip = GetClientIp();
                if (ip != UNKNOWN_IP && ip != "127.0.0.1")
                {
                    location = await GetLocationFromIpAsync(ip);
                }
                body += Environment.NewLine + $"From: {location}";

                int result = await Discord.SendMessage(request.Title, body);
                if (result == 0)
                {
                    var dbClient = new BootstersDbClient();
                    long cid = HasValidCustomerId() ? CustomerID.Value : 0;
                    var x = await dbClient.SaveEventAsync(BootstersDbClient.EventType.DiscordSent, cid, JsonConvert.SerializeObject(request));
                    return Ok("Message sent successfully.");
                }
                else
                    return InternalServerError(new System.Exception("Failed to send Discord message."));
            }
            catch (Exception ex)
            {
                // ✅ Log the exception securely
                try
                {
                    string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                    string logPath = Path.Combine(logDir, "discord-error.log");

                    Directory.CreateDirectory(logDir); // make sure directory exists
                    File.AppendAllText(logPath, DateTime.Now + " - " + ex.ToString() + Environment.NewLine);
                }
                catch
                {
                    // In case logging itself fails
                }

                return InternalServerError(); // generic failure message
            }
        }
    }
}
