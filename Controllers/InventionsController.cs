using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes;
using Ember2023.Classes.DBs;
using Ember2023.Auth;
using System.Text.Json;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/inventions")]
    [Route("/api/inventions")]
    public class InventionsController : ControllerBase
    {
        [HttpGet("v2/mine")]
        [HttpGet("v1/mine")]
        public IActionResult GetMyInventions()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var inventions = InventionsDB.GetInventionsByAccount((long)id)
                .Select(ToClientInvention)
                .ToList();
            return Ok(inventions);
        }

        [HttpGet("v1/featured")]
        public IActionResult GetFeaturedInventions() => Ok(Array.Empty<object>());

        private static object ToClientInvention(InventionsDB.Invention inv) => new
        {
            InventionId = inv.InventionId,
            Id = inv.InventionId,
            ReplicationId = inv.InventionId,
            Name = inv.Name,
            InventionName = inv.Name,
            Description = inv.Description,
            CreatorAccountId = inv.AccountId,
            CreatorPlayerId = inv.AccountId,
            CreatorId = inv.AccountId,
            RoomId = 0,
            ImageName = inv.ImageName,
            Image = inv.ImageName,
            PreviewImageName = inv.ImageName,
            DataBlob = inv.BlueprintJson,
            DataBlobName = "",
            DataBlobPath = "",
            DataBlobUrl = "",
            HasValidObjects = true,
            CanSpawn = true,
            Tags = new List<string>(),
            TagNames = new List<string>(),
            CurrentVersionNumber = Math.Max(1, inv.Version),
            Version = Math.Max(1, inv.Version),
            Accessibility = 1,
            IsPublished = inv.IsPublic,
            Published = inv.IsPublic,
            IsPublic = inv.IsPublic,
            IsFeatured = false,
            Price = 0,
            AllowTrial = true,
            CheerCount = 0,
            Uses = 0,
            NumDownloads = 0,
            CreatedAt = inv.CreatedAt,
            FirstPublishedAt = inv.CreatedAt,
            ModifiedAt = inv.UpdatedAt,
            UpdatedAt = inv.UpdatedAt,
            LastUpdated = inv.UpdatedAt
        };

        [HttpGet("v2/{inventionId}")]
        public IActionResult GetInvention(string inventionId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var invention = InventionsDB.GetInventionById((long)id, inventionId);
            if (invention == null)
                return NotFound();

            return Ok(new
            {
                inventionId = invention.InventionId,
                name = invention.Name,
                description = invention.Description,
                blueprintJson = invention.BlueprintJson,
                imageName = invention.ImageName,
                version = invention.Version,
                isPublic = invention.IsPublic,
                createdAt = invention.CreatedAt,
                updatedAt = invention.UpdatedAt
            });
        }

        [HttpPost("v2/save")]
        public IActionResult SaveInvention([FromBody] SaveInventionRequest request)
        {
            Console.WriteLine($"[Inventions] SaveInvention called");
            Console.WriteLine($"[Inventions] Request: {JsonSerializer.Serialize(request)}");
            
            var id = AuthStuff.GetPlayerId(Request);
            Console.WriteLine($"[Inventions] Player ID: {id}");
            if (id == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { error = "Name is required" });

            try
            {
                var invention = InventionsDB.SaveInvention(
                    (long)id,
                    request.Name,
                    request.Description ?? "",
                    request.BlueprintJson ?? "{}",
                    request.ImageName ?? "",
                    request.IsPublic
                );

                Console.WriteLine($"[Inventions] Saved invention: {invention.InventionId}");
                
                var response = new
                {
                    inventionId = invention.InventionId,
                    name = invention.Name,
                    description = invention.Description,
                    blueprintJson = invention.BlueprintJson,
                    imageName = invention.ImageName,
                    version = invention.Version,
                    isPublic = invention.IsPublic,
                    createdAt = invention.CreatedAt,
                    updatedAt = invention.UpdatedAt
                };
                
                Console.WriteLine($"[Inventions] Response: {JsonSerializer.Serialize(response)}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Inventions] ERROR: {ex.Message}");
                Console.WriteLine($"[Inventions] Stack: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("v2/{inventionId}")]
        public IActionResult DeleteInvention(string inventionId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var success = InventionsDB.DeleteInvention((long)id, inventionId);
            if (!success)
                return NotFound();

            return Ok(new { success = true });
        }

        [HttpPost("v2/spawn/{inventionId}")]
        public IActionResult SpawnInvention(string inventionId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var invention = InventionsDB.GetInventionById((long)id, inventionId);
            if (invention == null)
            {
                invention = InventionsDB.GetInvention(inventionId);
                if (invention == null || !invention.IsPublic)
                    return NotFound();
            }

            return Ok(new
            {
                inventionId = invention.InventionId,
                name = invention.Name,
                blueprintJson = invention.BlueprintJson,
                imageName = invention.ImageName,
                version = invention.Version
            });
        }

        public class SaveInventionRequest
        {
            public string Name { get; set; } = "";
            public string? Description { get; set; }
            public string? BlueprintJson { get; set; }
            public string? ImageName { get; set; }
            public bool IsPublic { get; set; }
        }
    }
}
