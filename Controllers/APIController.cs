using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes;
using Ember2023.Classes.DBs;
using Ember2023.Classes.DBs.DBClasses;
using PDBC = Ember2023.Classes.DBs.DBClasses.PlayerDBClasses;
using Ember2023.Auth;
using System.Text.Json;

namespace Ember2023.Controllers
{
    [ApiController]
    public class APIController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult GetNS()
        {
            string url = ServerConfig.BaseURL;
            return Ok(new
            {
                Accounts = url + "/acc",
                API = url,
                Auth = url + "/auth",
                BugReporting = url,
                Cards = url,
                CDN = url + "/cdn",
                Chat = url,
                Clubs = url,
                CMS = url,
                Commerce = url,
                Data = url,
                DataCollection = url,
                Discovery = url,
                Econ = url,
                GameLogs = url,
                Geo = url,
                Images = url + "/imageserver",
                Leaderboard = url,
                Link = url,
                Lists = url,
                Matchmaking = url + "/match",
                Moderation = url,
                Notifications = url + "/noti",
                PlatformNotifications = url,
                PlayerSettings = url,
                RoomComments = url,
                Rooms = url + "/roomserver",
                Storage = url,
                Strings = url,
                StringsCDN = url,
                Studio = url,
                Thorn = url,
                Videos = url,
                WWW = url
            });
        }

        [HttpGet("api/versioncheck/v4")]
        public IActionResult VersionCheck()
        {
            return Ok(new
            {
                ValidVersion = 0,
                VersionStatus = 0,
                UpdateNotificationStage = 0,
                IsVersionIslanded = false,
                IsCrossPlayDisabled = false
            });
        }

        [HttpGet("api/gameconfigs/v1/all")]
        public IActionResult GetGameConfigs()
        {
            string path = Path.Combine(Program.dataDir, "APIS", "GameConfigs.json");
            return System.IO.File.Exists(path) ? Content(System.IO.File.ReadAllText(path), "application/json") : NotFound();
        }

        [HttpGet("api/config/v1/amplitude")]
        public IActionResult GetAmplitude()
        {
            return Ok(new
            {
                AmplitudeKey = "cb2fb2ecb9953512c29af5bca58f2b4a",
                UseRudderStack = true,
                RudderStackKey = "23NiJHIgu3koaGNCZIiuYvIQNCu",
                UseStatSig = true,
                StatSigKey = "client-SBZkOrjD3r1Cat3f3W8K6sBd11WKlXZXIlCWj6l4Aje",
                StatSigEnvironment = 0
            });
        }

        [HttpGet("api/avatar/v1/defaultunlocked")]
        public IActionResult GetDefaultUnlocked()
        {
            return Ok(ServerConfig.Bracket);
        }

        [HttpGet("api/avatar/v1/defaultbaseavataritems")]
        public IActionResult GetDefaultBaseAvatarItems()
        {
            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/api/objectives/v1/myprogress")]
        public IActionResult GetMyObjectiveProgress()
        {
        	var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();
                
            return Ok(new 
            {
            	Objectives = new List<object>(),
                ObjectiveGroups = new List<object>()
            });
        }
        
        [HttpGet("/api/avatar/v2")]
        public IActionResult GetMyAvatar()
        {
            var player = AuthStuff.GetCurrentPlayer(Request);
            if (player == null)
            	return Unauthorized();

            return Ok(new
            {
                outfitSelections = player.Player.PlayerExtra.Avatar.OutfitSelections,
                faceFeatures = player.Player.PlayerExtra.Avatar.FaceFeatures,
                skinColor = player.Player.PlayerExtra.Avatar.SkinColor,
                hairColor = player.Player.PlayerExtra.Avatar.HairColor,
                avatarItems = player.Player.PlayerExtra.AvatarItems,
                savedAvatars = player.Player.PlayerExtra.SavedAvatars
            });
        }
        
        [HttpGet("/api/avatar/v2/items")]
        public IActionResult GetMyAvatarItemsPlayer()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(new
            {
                Results = player.Player.PlayerExtra.AvatarItems.Select(i => new { id = i }).ToList(),
                TotalResults = player.Player.PlayerExtra.AvatarItems.Count
            });
        }

        [HttpPost("/api/avatar/v2/items")]
        public IActionResult AddAvatarItem([FromBody] AvatarItemRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            if (!player.Player.PlayerExtra.AvatarItems.Contains(request.ItemId))
            {
                player.Player.PlayerExtra.AvatarItems.Add(request.ItemId);
                PlayerDB.Players.Update(player);
            }

            return Ok(new { success = true });
        }

        [HttpDelete("/api/avatar/v2/items/{itemId}")]
        public IActionResult RemoveAvatarItem(string itemId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            player.Player.PlayerExtra.AvatarItems.Remove(itemId);
            PlayerDB.Players.Update(player);

            return Ok(new { success = true });
        }

        [HttpGet("/api/avatar/v2/saved")]
        public IActionResult GetSavedOutfits()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(player.Player.PlayerExtra.SavedAvatars);
        }

        [HttpPost("/api/avatar/v2/saved")]
        public IActionResult SaveOutfit([FromBody] SavedOutfitRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return NotFound();

            var outfit = new PDBC.SavedOutfit
            {
                Slot = request.Slot,
                PreviewImageName = request.PreviewImageName,
                OutfitSelections = request.OutfitSelections,
                FaceFeatures = request.FaceFeatures,
                SkinColor = request.SkinColor,
                HairColor = request.HairColor
            };

            var existing = player.Player.PlayerExtra.SavedAvatars.FirstOrDefault(s => s.Slot == request.Slot);
            if (existing != null)
            {
                player.Player.PlayerExtra.SavedAvatars.Remove(existing);
            }
            player.Player.PlayerExtra.SavedAvatars.Add(outfit);
            PlayerDB.Players.Update(player);

            return Ok(new { success = true });
        }

        public class AvatarItemRequest
        {
            public string ItemId { get; set; } = "";
        }

        public class SavedOutfitRequest
        {
            public int Slot { get; set; }
            public string? PreviewImageName { get; set; }
            public string? OutfitSelections { get; set; }
            public string? FaceFeatures { get; set; }
            public string? SkinColor { get; set; }
            public string? HairColor { get; set; }
        }
        
        [HttpPost("/api/avatar/v2/set")]
        public IActionResult SetMyAvatar([FromBody] PlayerDBClasses.Avatar request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            return Ok(PlayerDB.SetAvatar((long)id, request));
        }
        
        [HttpGet("/api/avatar/v4/items")]
        public IActionResult GetMyAvatarItems()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            string path = Path.Combine(Program.dataDir, "APIS", "Items", "AvatarItems.json");
            return System.IO.File.Exists(path) ? Content(System.IO.File.ReadAllText(path), "application/json") : NotFound();
        }
        
        [HttpGet("/api/PlayerReporting/v1/moderationBlockDetails")]
        public IActionResult GetMyModerationBlockDetails()
        {
            var player = AuthStuff.GetCurrentPlayer(Request);
            if (player == null)
            	return Unauthorized();

            return Ok(player.Player.PlayerExtra.ModerationBlockDetails);
        }
        
        [HttpGet("/api/relationships/v2/get")]
        public IActionResult GetMyRelationships()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/api/messages/v2/get")]
        public IActionResult GetMyMessages()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/playersettings")]
        public IActionResult GetMySettings()
        {
            var player = AuthStuff.GetCurrentPlayer(Request);
            if (player == null)
            	return Unauthorized();

            return Ok(player.Player.PlayerExtra.Settings);
        }
        
        [HttpPut("/playersettings")]
        public IActionResult SetMySettings([FromForm] string key, [FromForm] string value)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();
                
			PlayerDB.SetPlayerSetting(key, value ?? "", (long)id);
            
            return NoContent();
        }
        
        [HttpGet("/econ/customAvatarItems/v1/owned")]
        public IActionResult GetMyOwnedCustomAvatarItems([FromQuery] int skip, [FromQuery] int take)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();
                
            return Ok(new 
            {
            	Results = Array.Empty<object>(),
                TotalResults = 0
            });
        }
        
        [HttpGet("/api/checklist/v1/current")]
        public IActionResult GetMyChecklist()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/api/players/v2/progression/bulk")]
        public IActionResult GetProgressionForPlayers([FromQuery] List<long> id)
        {
            var authId = AuthStuff.GetPlayerId(Request);
            if (authId == null) 
                return Unauthorized();

            if (id == null || id.Count == 0)
                return Ok(new List<PlayerDBClasses.PlayerProgressionDTO>());

            var progressions = PlayerDB.GetProgressionBulk(id);

            return Ok(progressions);
        }
        
        [HttpGet("/api/playerReputation/v2/bulk")]
        public IActionResult GetReputationBulk([FromQuery] List<long> id)
        {
            var authId = AuthStuff.GetPlayerId(Request);
            if (authId == null) 
                return Unauthorized();

            if (id == null || id.Count == 0)
                return Ok(new List<PlayerDBClasses.Reputation>());

            var results = PlayerDB.GetReputationBulk(id);
            return Ok(results);
        }
        
        [HttpPost("/api/PlayerReporting/v1/hile")] // todo log to channel
        public IActionResult PlayerReportingHile()
        {
            var authId = AuthStuff.GetPlayerId(Request);
            if (authId == null) 
                return Unauthorized();
                
            return Ok(false);
        }
        
        [HttpGet("api/config/v2")]
        public IActionResult GetConfigV2()
        {
            string path = Path.Combine(Program.dataDir, "APIS", "ConfigV2.json");
            return System.IO.File.Exists(path) ? Content(System.IO.File.ReadAllText(path), "application/json") : NotFound();
        }
        
        [HttpGet("/api/announcement/v1/get")]
        [HttpGet("/api/PlayerReporting/v1/voteToKickReasons")]
        [HttpGet("/api/avatar/v3/saved")]
        [HttpGet("/api/equipment/v2/getUnlocked")]
        [HttpGet("/api/consumables/v2/getUnlocked")]
        [HttpGet("/api/images/v2/named")]
        [HttpGet("/api/avatar/v2/gifts")]
        [HttpGet("/api/gamerewards/v1/pending")]
        [HttpGet("/api/roomkeys/v1/mine")]
        [HttpGet("/api/roomcurrencies/v1/currencies")]
        public IActionResult TodoImplement()
        {
            return Ok(ServerConfig.Bracket);
        }
        
        [HttpGet("/cdn/config/LoadingScreenTipData")]
        [HttpGet("/api/loadingscreen/v1/config")]
        public IActionResult GetLoadingScreenConfig()
        {
            var path = Path.Combine(Program.dataDir, "Imports", "loadingscreen.rewritten.json");
            if (System.IO.File.Exists(path))
            {
                return Content(System.IO.File.ReadAllText(path), "application/json");
            }
            return NotFound();
        }
        
        [HttpGet("/api/playerevents/v1/all")]
        public IActionResult GetAllPlayerEvents()
        {
            return Ok(new 
            {
            	Created = Array.Empty<object>(),
                Responses = Array.Empty<object>()
            });
        }
        
        [HttpGet("/api/customAvatarItems/v1/isCreationAllowedForAccount")]
        public IActionResult GetCustomAvatarItemsIsCreationAllowedForAccount()
        {
            return Ok(new
            {
            	success = true,
                value = (object?)null
            });
        }
        
        [HttpGet("/api/customAvatarItems/v1/isCreationEnabled")]
        [HttpGet("/api/customAvatarItems/v1/isRenderingEnabled")]
        public IActionResult CustomAvatarItemsIsEnabled()
        {
            return Ok(true);
        }

        [HttpGet("/econ/customAvatarItems/v1/owned")]
        [HttpGet("/api/customAvatarItems/v1/owned")]
        public IActionResult GetOwnedCustomAvatarItems()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = PlayerDB.Players.FindById((long)id);
            if (player == null || player.Player == null)
                return Ok(new { Results = new List<object>(), TotalResults = 0 });

            return Ok(new
            {
                Results = player.Player.PlayerExtra.AvatarItems.Select(i => new
                {
                    customAvatarItemId = i,
                    itemId = i,
                    instanceId = i,
                    name = "Custom Item",
                    description = "",
                    imageName = "",
                    rarity = "Common",
                    type = "Clothing"
                }).ToList(),
                TotalResults = player.Player.PlayerExtra.AvatarItems.Count
            });
        }
        
        [HttpGet("/api/roomconsumables/v1/roomConsumable/room/{roomId}")]
        public IActionResult GetRoomConsumablesForRoom(long roomId)
        {
        	return Ok(ServerConfig.Bracket);
        }
        
        [HttpPost("/api/sanitize/v1")]
        public IActionResult SanitizeV1([FromBody] SanitizeRequest request)
        {
        	return Ok(JsonSerializer.Serialize(request.Value));
        }
        
        [HttpPost("/api/sanitize/v1/isPure")]
        public IActionResult SanitizeV1IsPure()
        {
        	return Ok(new
            {
                IsPure = true
            });
        }

        [HttpPut("/api/players/v1/{accountId}/bio")]
        public IActionResult SetBio(long accountId, [FromBody] BioRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            if (id != accountId)
                return Forbid();

            var player = PlayerDB.Players.FindById(accountId);
            if (player == null || player.Player == null)
                return NotFound();

            player.Player.Bio = request.Bio ?? "";
            PlayerDB.Players.Update(player);

            return NoContent();
        }

        [HttpGet("/api/players/v1/{accountId}/bio")]
        public IActionResult GetBio(long accountId)
        {
            var player = PlayerDB.Players.FindById(accountId);
            if (player == null || player.Player == null)
                return NotFound();

            return Ok(new { bio = player.Player.Bio ?? "" });
        }

        public class BioRequest
        {
            public string? Bio { get; set; }
        }
        
        public class SanitizeRequest
        {
            public string Value { get; set; } = string.Empty;
        }
    }
}