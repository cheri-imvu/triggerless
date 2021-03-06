using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Triggerless.API.Controllers
{
    public class AvatarCardController : ApiController {

        private HttpResponseMessage GetResponse(string content) {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new StringContent(content, Encoding.UTF8, "application/json");
            return response;
        }

        // GET api/<controller>/5
        [Route("api/avatarcard/{idOrName}")]
        public HttpResponseMessage Get(string idOrName) {

            // osCsid is the *.imvu.com Cookie that identifies your avatar as being "logged in".
            // I won't show you mine, because you could destroy my account if I revealed it.
            // also, it might change with time, so I keep it in web.config as an AppSetting.
            // So I'll let people use my osCsid for now.

            string osCsid = ConfigurationManager.AppSettings["osCsid"];
            int id;
            string json;
            WebClient wc;
            const string FAKE_BROWSER_NAME = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";

            // The way to find out if this is an id (a number) or a name (a string) is TryParse
            // If idOrName is a number, TryParse will return true and set the value of id to the customer id
            // Otherwise it's a string, and we have to use the IMVU User API for avatar names to get the id

            if (!int.TryParse(idOrName, out id))
            {

                // We have to call the User API for avatar names, so we set up a new WebClient
                // to make the remote call. We'll only accept application/json content, we'll spoof
                // a popular web browser via UserAgent, add our IMVU login cookie, and make sure we're
                // using a secure connection via HTTPS.

                wc = new WebClient();
                string name = idOrName;
                wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
                wc.Headers.Add(HttpRequestHeader.UserAgent, FAKE_BROWSER_NAME);
                wc.Headers.Add(HttpRequestHeader.Cookie, osCsid);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Now let's make a synchronous HTTP call to the User API, because we can't return anything
                // until this completes.

                try
                {
                    json = wc.DownloadString("https://api.imvu.com/users/avatarname/" + name);
                }
                catch (Exception)
                {
                    // If there was an error or no response, we will "rethrow" the
                    // error back to the client with status as failure

                    string responseContent = JsonConvert.SerializeObject(new { status = "failure", message = "Error finding Avatar ID" });
                    return GetResponse(responseContent);
                }

                // At this point we have the string response. We can deserialize the string using Newtonsoft.Json's JObject
                // and if the status property is not "success", rethrow the error back to the client.

                JObject jObject = JObject.Parse(json);
                if (jObject["status"].Value<string>() != "success")
                {
                    return GetResponse(json);
                }

                // For some silly reason, IMVU decided to bury the data into a "denormalized" property.
                // However, you can drill into the JSON using JObject much like navigating an XML document
                // and find the customer_id

                id = jObject["denormalized"].First.First["data"]["customer_id"].Value<int>();
            }

            // The next part is straightforward, since we already have the customer id and we don't
            // have to parse any JSON, just return it back to the client. It will have a status of "success" or "failure"
            // and we don't have to deal with that here. One thing to note is that this response will be in XML format
            // if we don't explicitly set the HTTP Accept header to application/json, and we don't want that.
            // Also, we have to set the security protocol back to 0 for unencrypted HTTP.

            wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Accept, "application/json");
            wc.Headers.Add(HttpRequestHeader.UserAgent, FAKE_BROWSER_NAME);
            wc.Headers.Add(HttpRequestHeader.Cookie, "osCsid=" + osCsid);  // turns out this is necessary to get show_ap
            ServicePointManager.SecurityProtocol = 0;
            json = wc.DownloadString("http://www.imvu.com/api/avatarcard.php?cid=" + id + "&source=web");
            try
            {
                JObject avatarCard = JObject.Parse(json);
                avatarCard["status"] = "success";
                return GetResponse(avatarCard.ToString());
            }
            catch (Exception exc)
            {
                return GetResponse(
                    JsonConvert.SerializeObject(
                        new { status = "failure", message = "Unable to parse JSON", exception = exc.Message }
                    )
                );
            }
        }


    }
}