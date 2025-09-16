using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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

        [Route("api/product/sounds/{productId}")]
        public async Task<HttpResponseMessage> GetProductSounds(long productId)
        {
            using (var client = new ImvuApiClient()) 
            {
                return GetJsonResponseFromObject(await client.GetProductSounds(productId));
            }            
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

        [HttpGet, Route("api/productsounds/{p}")]
        public async Task<HttpResponseMessage> ProductSounds([FromUri] long p)
        {
            using (var client = new ImvuApiClient())
            {
                var result = await client.GetProductSoundTriggerPayload(p);
                return GetJsonResponseFromObject(result);
            }
        }

        [HttpPost, Route("api/outfits")]
        public async Task<ExposeOutfitsResponse> GetOutfits([FromBody] ExposeOutfitsRequest req)
        {
            //var req = ImvuApiClient.RequestFromUrl(url);
            using (var client = new ImvuApiClient())
            {
                return await client.GetOutfits(req);
            }
        }

        [HttpPost, Route("api/outfit")]
        public async Task<ExposeOutfitsResponseEntry> GetOutfit([FromBody] ExposeOutfitsRequestEntry req)
        {
            //var req = ImvuApiClient.RequestFromUrl(url);
            using (var client = new ImvuApiClient())
            {
                return await client.GetOutfit(req);
            }
        }

        [HttpGet, Route("api/product/template/{p}")]
        public async Task<HttpResponseMessage> GetTemplate([FromUri] long p)
        {
            using (var client = new ImvuApiClient())
            {
                var result = await client.GetTemplate(p);
                return GetJsonResponseFromObject(result);
            }
        }

    }
}

