using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Triggerless.API.Controllers
{
    public class TechSupportController : ApiController
    {
        [HttpPost]
        [Route("api/techsupport")]
        public async Task<IHttpActionResult> TechSupport()
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("Unsupported media type.");

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            string username = null;
            byte[] fileBytes = null;
            string filename = null;

            foreach (var content in provider.Contents)
            {
                var disposition = content.Headers.ContentDisposition;
                if (disposition.Name.Trim('"') == "username")
                {
                    username = await content.ReadAsStringAsync();
                }
                else if (disposition.Name.Trim('"') == "file")
                {
                    fileBytes = await content.ReadAsByteArrayAsync();
                    filename = disposition.FileName?.Trim('"');
                }
            }

            if (fileBytes == null || filename == null || username == null)
                return BadRequest("Missing file or username.");

            // Save file
            var folderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UploadedFiles");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var savePath = Path.Combine(folderPath, $"{username}_{filename}");
            File.WriteAllBytes(savePath, fileBytes);

            return Ok("File uploaded successfully.");
        }
    }
}
