using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Triggerless.API.Controllers
{
    public class BaseController: ApiController
    {
        protected HttpResponseMessage GetJsonResponse(string json)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        protected HttpResponseMessage GetJsonResponseFromObject(object thing)
        {
            return GetJsonResponse(JsonConvert.SerializeObject(thing));
        }

        protected HttpResponseMessage GetBinaryResponse(byte[] bytes, string filename, string mediaType = "application/octet-stream")
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new ByteArrayContent(bytes);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            response.Content.Headers.ContentDisposition =
                ContentDispositionHeaderValue.Parse($"attachment; filename = \"{filename}\"");
            return response;
        }

    }
}