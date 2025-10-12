using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{

    [RoutePrefix("api/bot")]
    public class TriggerbotController : CustomerController
    {
        [Route("event/{eventId:short}"), HttpPost]
        public async Task<IHttpActionResult> RecordEvent(short eventId, [FromBody] string jsonText)
        {
            IHttpActionResult result = Ok("Nothing yet");
            var location = "Internal Network";
            var ip = GetClientIp();
            if (ip != UNKNOWN_IP && ip != "127.0.0.1")
            {
                try
                {
                    location = await GetLocationFromIpAsync(ip);
                }
                catch (Exception)
                {
                    // don't stop for this burp
                    location = "Unknown Location";
                }
            }
            var cid = CustomerID ?? 0;
            BootstersDbClient.EventType eventType = (BootstersDbClient.EventType)eventId;

            var dbClient = new BootstersDbClient();
            BootstersDbClient.EventResult dbResult = null;
            Exception exception = null;
            try
            {
                dbResult = await dbClient.SaveEventAsync(eventType, cid, jsonText);
            }
            catch (Exception ex) 
            { 
                dbResult = new BootstersDbClient.EventResult() { 
                    Message = ex.Message, 
                    Type = BootstersDbClient.EventResultType.Fail 
                };
                exception = ex; 
            }

            if (dbResult.Type == BootstersDbClient.EventResultType.Success)
            {
                result = Ok(dbResult);
            }
            else {
                result = InternalServerError(exception);
            }
            return result;
        }
    }
}
