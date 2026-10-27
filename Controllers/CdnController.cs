using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/cdn")]
    public class CdnController : ControllerBase
    {
        [HttpGet("room/{*blobName}")]
        public async Task<IActionResult> GetRoomBlob(string blobName)
        {
            blobName = Uri.UnescapeDataString(blobName ?? "").TrimStart('/');

            string baseRoomPath = Path.Combine(Program.dataDir, "CDN", "room");

            string directPath = Path.Combine(baseRoomPath, blobName);
            if (System.IO.File.Exists(directPath))
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(directPath);
                return File(bytes, "application/octet-stream");
            }

            // Also try without .blob extension
            string withBlob = blobName + ".blob";
            string directPath2 = Path.Combine(baseRoomPath, withBlob);
            if (System.IO.File.Exists(directPath2))
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(directPath2);
                return File(bytes, "application/octet-stream");
            }

            return NotFound();
        }
    }
}
