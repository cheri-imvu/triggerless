using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Triggerless.API.Controllers
{
    [RoutePrefix("api/upload")]
    public class FileUploadController : ApiController
    {
        [HttpPost]
        [Route("techsupport")]
        public async Task<IHttpActionResult> UploadTechSupport()
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("Expected multipart content.");

            var uploadPath = HttpContext.Current.Server.MapPath("~/App_Data/Uploads");
            Directory.CreateDirectory(uploadPath);

            var provider = new MultipartFormDataStreamProvider(uploadPath);
            await Request.Content.ReadAsMultipartAsync(provider);

            foreach (var fileData in provider.FileData)
            {
                var fileName = fileData.Headers.ContentDisposition.FileName?.Trim('"') ?? "unknown.zip";
                var tempPath = fileData.LocalFileName;
                var finalPath = Path.Combine(uploadPath, fileName);
                File.Move(tempPath, finalPath);
            }

            return Ok("Upload completed");
        }
    }
}
