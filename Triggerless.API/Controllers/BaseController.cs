using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Triggerless.API.Controllers
{
    public class BaseController: ApiController
    {
        protected HttpResponseMessage GetResponse(string content)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Content = new StringContent(content, Encoding.UTF8, "application/json");
            return response;
        }

        protected HttpResponseMessage GetResponseFromObject(object thing)
        {
            return GetResponse(JsonConvert.SerializeObject(thing));
        }

    }
}