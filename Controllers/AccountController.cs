using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes;
using Ember2023.Classes.DBs;
using PDBC = Ember2023.Classes.DBs.DBClasses.PlayerDBClasses;
using Ember2023.Auth;

namespace Ember2023.Controllers
{
    public class RegisterRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    [ApiController]
    [Route("/acc")]
    public class AccountController : ControllerBase
    {
        [HttpGet("account/bulk")]
        public IActionResult GetAccountsBulk([FromQuery] List<long> id)
        {
            return Ok(PlayerDB.GetAccountsBulk(id));
        }

        [HttpGet("account/me")]
        public IActionResult GetAccountMe()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var account = PlayerDB.GetAccountMe(id.Value);
            return account != null ? Ok(account) : NotFound();
        }

        [HttpPost("account/register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) ||
                string.IsNullOrWhiteSpace(req.Password) ||
                string.IsNullOrWhiteSpace(req.DisplayName))
            {
                return BadRequest(new { error = "Missing required fields" });
            }

            if (req.Password.Length < 4)
            {
                return BadRequest(new { error = "Password too short" });
            }

            // Check if username is taken
            var existing = PlayerDB.Players.Find(x =>
                x.Player != null &&
                x.Player.Username != null &&
                x.Player.Username.Equals(req.Username, StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

            if (existing != null)
            {
                return BadRequest(new { error = "Username already taken" });
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var newPlayerData = new PDBC.Player
            {
                Username = req.Username,
                DisplayName = req.DisplayName,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                ProfileImage = "DefaultPFP.png",
                AvailableUsernameChanges = 3,
                IsJunior = false,
                Level = 1,
                XP = 0,
                PlayerExtra = new PDBC.PlayerExtra
                {
                    Settings = new List<PDBC.Setting>
                    {
                        new PDBC.Setting { Key = "Recroom.AccountCreation.HasStarted", Value = "True" },
                        new PDBC.Setting { Key = "Recroom.AccountCreation.HasChosenUsername", Value = "True" },
                        new PDBC.Setting { Key = "Recroom.AccountCreation.HasCreatedPassword", Value = "True" },
                        new PDBC.Setting { Key = "Recroom.AccountCreation.HasFinished", Value = "True" },
                        new PDBC.Setting { Key = "TUTORIAL_COMPLETE_MASK", Value = "57" }
                    }
                }
            };

            var newFullPlayer = new PDBC.FullPlayer
            {
                PlatformIds = new List<PDBC.mPlatformID>(),
                Player = newPlayerData,
                PlayerRoles = new List<PDBC.PlayerRoles> { PDBC.PlayerRoles.Developer },
                AuthToken = Guid.NewGuid().ToString(),
                Password = hash
            };

            PlayerDB.Players.Insert(newFullPlayer);

            Console.WriteLine($"[REGISTER] Created account {newFullPlayer.PlayerId}: {req.Username}");

            return Ok(new
            {
                accountId = newFullPlayer.PlayerId,
                username = newFullPlayer.Player.Username,
                displayName = newFullPlayer.Player.DisplayName,
                message = "Account created successfully"
            });
        }

        [HttpPost("account/login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            {
                return BadRequest(new { error = "Missing username or password" });
            }

            var account = PlayerDB.Players.Find(x =>
                x.Player != null &&
                x.Player.Username != null &&
                x.Player.Username.Equals(req.Username, StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

            if (account == null)
            {
                return BadRequest(new { error = "Invalid username or password" });
            }

            if (string.IsNullOrEmpty(account.Password) ||
                !BCrypt.Net.BCrypt.Verify(req.Password, account.Password))
            {
                return BadRequest(new { error = "Invalid username or password" });
            }

            // Update last login
            account.Player.LastLoginAt = DateTime.UtcNow;
            PlayerDB.Players.Update(account);

            Console.WriteLine($"[LOGIN] Account {account.PlayerId}: {account.Player.Username}");

            return Ok(new
            {
                accountId = account.PlayerId,
                username = account.Player.Username,
                displayName = account.Player.DisplayName,
                authToken = account.AuthToken,
                message = "Login successful"
            });
        }
    }
}
