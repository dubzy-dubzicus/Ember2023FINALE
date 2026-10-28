using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes.DBs;
using Ember2023.Auth;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/recnet")]
    public class RecNetController : ControllerBase
    {
        [HttpGet("photoCheers")]
        public IActionResult GetPhotoCheers([FromQuery] string photoPath)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            int count = RecNetDB.CountPhotoCheers(photoPath);
            bool hasCheer = RecNetDB.HasPhotoCheer(photoPath, id.Value);

            return Ok(new
            {
                cheerCount = count,
                hasCheer = hasCheer
            });
        }

        [HttpPost("photoCheers")]
        public IActionResult SetPhotoCheer([FromBody] PhotoCheerRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            bool success = RecNetDB.SetPhotoCheer(request.PhotoPath, id.Value, request.Cheered);
            return Ok(new { success = success });
        }

        [HttpGet("photoComments")]
        public IActionResult GetPhotoComments([FromQuery] string photoPath)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var comments = RecNetDB.PhotoComments.Find(c =>
                c.PhotoPath == RecNetDB.NormalizePhotoPath(photoPath))
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return Ok(new
            {
                Results = comments.Select(c => new
                {
                    id = c.Id,
                    accountId = c.AccountId,
                    text = c.Text,
                    createdAt = c.CreatedAt
                }).ToList(),
                TotalResults = comments.Count
            });
        }

        [HttpPost("photoComments")]
        public IActionResult AddPhotoComment([FromBody] PhotoCommentRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var comment = new RecNetDB.PhotoComment
            {
                PhotoPath = RecNetDB.NormalizePhotoPath(request.PhotoPath),
                AccountId = id.Value,
                Text = request.Text,
                CreatedAt = DateTime.UtcNow
            };
            RecNetDB.PhotoComments.Insert(comment);

            return Ok(new
            {
                id = comment.Id,
                accountId = comment.AccountId,
                text = comment.Text,
                createdAt = comment.CreatedAt
            });
        }

        [HttpGet("savedImages")]
        public IActionResult GetSavedImages()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var images = RecNetDB.SavedImages.Find(i => i.AccountId == id.Value)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            return Ok(new
            {
                Results = images.Select(i => new
                {
                    photoPath = i.PhotoPath,
                    imageId = i.ImageId,
                    roomId = i.RoomId,
                    lookupName = i.LookupName,
                    createdAt = i.CreatedAt
                }).ToList(),
                TotalResults = images.Count
            });
        }

        public class PhotoCheerRequest
        {
            public string PhotoPath { get; set; } = "";
            public bool Cheered { get; set; }
        }

        public class PhotoCommentRequest
        {
            public string PhotoPath { get; set; } = "";
            public string Text { get; set; } = "";
        }
    }
}
