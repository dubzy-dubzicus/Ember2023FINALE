using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Ember2023.Auth;
using System;
using System.IO;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/cdn")]
    public class CDNController : ControllerBase
    {
        private static readonly string BlobRoot = Path.Combine(Program.dataDir, "CDN");

        static CDNController()
        {
            Directory.CreateDirectory(Path.Combine(BlobRoot, "room"));
            Directory.CreateDirectory(Path.Combine(BlobRoot, "invention"));
            Directory.CreateDirectory(Path.Combine(BlobRoot, "avatar"));
            Directory.CreateDirectory(Path.Combine(BlobRoot, "temp"));
        }

        [HttpGet("blob/{type}/{blobId}")]
        public IActionResult GetBlob(string type, string blobId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            string typeDir = type.ToLower() switch
            {
                "room" => "room",
                "invention" => "invention",
                "avatar" => "avatar",
                _ => "temp"
            };

            string filePath = Path.Combine(BlobRoot, typeDir, blobId);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);
            string mime = Path.GetExtension(filePath).ToLower() switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".json" => "application/json",
                _ => "application/octet-stream"
            };

            Response.Headers.Append("Cache-Control", "public, max-age=86400");
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            return File(bytes, mime);
        }

        [HttpPost("blob/{type}")]
        public IActionResult UploadBlob(string type, [FromForm] IFormFile file)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file provided" });

            string typeDir = type.ToLower() switch
            {
                "room" => "room",
                "invention" => "invention",
                "avatar" => "avatar",
                _ => "temp"
            };

            string dirPath = Path.Combine(BlobRoot, typeDir);
            Directory.CreateDirectory(dirPath);

            string blobId = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(dirPath, blobId);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok(new
            {
                success = true,
                blobId = blobId,
                type = type,
                url = $"/cdn/blob/{type}/{blobId}",
                size = file.Length
            });
        }

        [HttpDelete("blob/{type}/{blobId}")]
        public IActionResult DeleteBlob(string type, string blobId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            string typeDir = type.ToLower() switch
            {
                "room" => "room",
                "invention" => "invention",
                "avatar" => "avatar",
                _ => "temp"
            };

            string filePath = Path.Combine(BlobRoot, typeDir, blobId);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            System.IO.File.Delete(filePath);

            return Ok(new { success = true, deletedBlobId = blobId });
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var stats = new Dictionary<string, object>();
            foreach (var type in new[] { "room", "invention", "avatar", "temp" })
            {
                string dirPath = Path.Combine(BlobRoot, type);
                if (Directory.Exists(dirPath))
                {
                    var files = Directory.GetFiles(dirPath);
                    long totalSize = 0;
                    foreach (var f in files)
                    {
                        try { totalSize += new FileInfo(f).Length; } catch { }
                    }
                    stats[type] = new { fileCount = files.Length, totalSizeBytes = totalSize };
                }
                else
                {
                    stats[type] = new { fileCount = 0, totalSizeBytes = 0 };
                }
            }

            return Ok(new
            {
                blobRoot = BlobRoot,
                types = stats
            });
        }
    }
}
