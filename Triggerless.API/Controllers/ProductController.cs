using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Triggerless.Models;
using Triggerless.Services;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class ProductController : BaseController
    {
        [Route("api/product/{productId}")]
        public async Task<HttpResponseMessage> Get(long productId)
        {
            return GetJsonResponseFromObject(await GetProduct(productId));
        }

        public async Task<ImvuProduct> GetProduct(long productId) {
            using (var client = new ImvuApiClient())
            {
                return await client.GetProduct(productId);
            }
            
        }

        [HttpGet, Route("api/products")]
        public async Task<HttpResponseMessage> Products([FromUri] long[] p)
        {
            return GetJsonResponseFromObject(await GetProducts(p));
        }
        public async Task<ImvuProductList> GetProducts(long[] p)
        {
            using (var client = new ImvuApiClient())
            {
                return await client.GetProducts(p);
            }
            
        }
    }
}
