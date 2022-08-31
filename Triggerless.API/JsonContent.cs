using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Triggerless.API
{
    public class JsonContent: StringContent
    {
        public JsonContent(object data): base(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json") {}
    }
}