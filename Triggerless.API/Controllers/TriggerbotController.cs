using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web.Http;
using Triggerless.Services.Server;
using static Triggerless.Services.Server.BootstersDbClient;

namespace Triggerless.API.Controllers
{

    [RoutePrefix("api/bot")]
    public class TriggerbotController : CustomerController
    {
        [Route("event/{eventId}"), HttpPost]
        public async Task<IHttpActionResult> RecordEvent(short eventId)
        {
            var jsonText = await Request.Content.ReadAsStringAsync(); // raw body

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



        [Route("events/{page}")]
        public async Task<IHttpActionResult> Page(int page = 0)
        {
            throw new NotImplementedException();
            /*
            var dbClient = new BootstersDbClient();
            BootstersDbClient.EventResult dbResult = null;
            Exception exception = null;
            try
            {
                dbResult = await dbClient.SaveEventAsync(eventType, cid, jsonText);
            }
            catch (Exception ex)
            {
                dbResult = new BootstersDbClient.EventResult()
                {
                    Message = ex.Message,
                    Type = BootstersDbClient.EventResultType.Fail
                };
                exception = ex;
            }
            */
        }
    }
}
