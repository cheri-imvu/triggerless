using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Triggerless.Services.Server;

namespace Triggerless.API.Controllers
{
    public class OutfitModel
    {
        public long CustomerId { get; set; }
        public string Request { get; set; }
    }   
    public class OutfitsController : BaseController
    {
        [HttpPost]
        [Route("api/outfits/request")]
        public async Task<HttpResponseMessage> OutfitsRequest([FromBody] OutfitModel request)
        {
            if (request == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Data is required.");
            }

            if (await new BootstersDbClient().SaveOutfitsRequest(request.CustomerId, request.Request))
            {
                return GetJsonResponseFromObject(new
                {
                    CustomerId = request.CustomerId,
                    Status = "Success",
                });

            }
            return GetJsonResponseFromObject(new
            {
                CustomerId = request.CustomerId,
                Status = "Failed",
            });
        }
    }
}
