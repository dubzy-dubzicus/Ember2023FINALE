using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes;
using Ember2023.Classes.DBs;
using PDBC = Ember2023.Classes.DBs.DBClasses.PlayerDBClasses;
using PlayerDBClasses = Ember2023.Classes.DBs.DBClasses.PlayerDBClasses;
using Ember2023.Auth;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        [HttpGet("eac/challenge")]
        public IActionResult GetEACChallenge()
        {
            return Ok("\"e\"");
        }

        [HttpGet("cachedlogin/forplatformid/{platform}/{platformId}")]
        public IActionResult GetCachedLogins(PlayerDBClasses.Platforms platform, ulong platformId)
        {
            if (PlayerDB.GetLogins(platform, platformId, out var accounts) && accounts.Count > 0)
            {
                return Ok(accounts);
            }

            var newPlayer = PlayerDB.CreateAccount(platform, platformId, false);

            var newCachedLogin = new List<PlayerDBClasses.CachedLogins>
            {
                new PlayerDBClasses.CachedLogins
                {
                    accountId = newPlayer.PlayerId,
                    lastLoginTime = newPlayer.Player.LastLoginAt,
                    platform = platform,
                    platformId = platformId.ToString(),
                    requirePassword = false
                }
            };

            return Ok(newCachedLogin);
        }

        [HttpPost("connect/token")]
        [HttpPost("connect/gametoken")]
        public async Task<IActionResult> ConnectToken()
        {
            if (!Request.HasFormContentType)
            {
                return BadRequest(new { error = "invalid_request", error_description = "Expected form data." });
            }

            var form = await Request.ReadFormAsync();

            string grantType =
                FirstFormValue(form, "grant_type", "grantType")
                    .Trim()
                    .Replace("-", "_", StringComparison.Ordinal)
                    .ToLowerInvariant();

            Console.WriteLine($"[AUTH TOKEN] grant={grantType} fields={string.Join(",", form.Keys.OrderBy(k => k))}");

            bool isFreshLogin = grantType is "cached_login" or "cachedlogin" or "password" or "create_account" or "createaccount";

            if (isFreshLogin)
            {
                IActionResult gateResult = CheckModGate(form);
                if (gateResult != null)
                {
                    return gateResult;
                }
            }

            switch (grantType)
            {
                case "cached_login":
                case "cachedlogin":
                    return HandleCachedLoginGrant(form);

                case "password":
                    return HandlePasswordGrant(form);

                case "create_account":
                case "createaccount":
                    return HandleCreateAccountGrant(form);

                case "refresh_token":
                case "refreshtoken":
                    return HandleRefreshTokenGrant(form);

                default:
                    Console.WriteLine($"[AUTH TOKEN FAILED] Unsupported grant type: {grantType}");
                    return OAuthError("unsupported_grant_type", $"Unsupported grant_type: {grantType}");
            }
        }

        private IActionResult HandleCachedLoginGrant(IFormCollection form)
        {
            if (!TryReadAccountId(form, out long accountId))
            {
                return OAuthError("invalid_request", "account_id is required.");
            }

            var player = PlayerDB.Players.FindById(accountId);

            if (player?.Player == null)
            {
                return OAuthError("invalid_grant", "Account not found.");
            }

            return Login(player, logAsLogin: true);
        }

        private IActionResult HandlePasswordGrant(IFormCollection form)
        {
            string username =
                FirstFormValue(form, "username", "user_name", "userName")
                    .Trim()
                    .TrimStart('@');

            string password =
                FirstFormValue(form, "password");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                return OAuthError("invalid_request", "username and password are required.");
            }

            var account = PlayerDB.Players.Find(x =>
                x.Player != null &&
                x.Player.Username != null &&
                x.Player.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
            ).FirstOrDefault();

            if (account == null)
            {
                return OAuthError("invalid_username_or_password", "Username and password do not match.");
            }

            if (string.IsNullOrEmpty(account.Password) ||
                !BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                return OAuthError("invalid_username_or_password", "Username and password do not match.");
            }

            account.Player.LastLoginAt = DateTime.UtcNow;
            PlayerDB.Players.Update(account);

            Console.WriteLine($"[LOGIN] Account {account.PlayerId}: {account.Player.Username}");

            return Login(account, logAsLogin: true);
        }

        private IActionResult HandleCreateAccountGrant(IFormCollection form)
        {
            if (!TryReadPlatform(form, out var platform, out var platformId))
            {
                return OAuthError("invalid_request", "A valid platform and platform_id are required.");
            }

            var existingPlayer = PlayerDB.Players
                .FindAll()
                .FirstOrDefault(candidate =>
                    candidate.PlatformIds?.Any(identity =>
                        identity.Platform == platform &&
                        identity.PlatformId == platformId)
                    == true);

            if (existingPlayer?.Player != null)
            {
                return OAuthError("account_limit_reached", "This platform account is already registered.");
            }

            var newPlayerData = new PDBC.Player
            {
                Username = FirstFormValue(form, "username", "user_name", "userName").Trim(),
                DisplayName = FirstFormValue(form, "display_name", "displayName").Trim(),
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

            var hash = BCrypt.Net.BCrypt.HashPassword(FirstFormValue(form, "password"));

            var newFullPlayer = new PDBC.FullPlayer
            {
                PlatformIds = new List<PDBC.mPlatformID>
                {
                    new PDBC.mPlatformID { Platform = platform, PlatformId = platformId }
                },
                Player = newPlayerData,
                PlayerRoles = new List<PDBC.PlayerRoles> { PDBC.PlayerRoles.Developer },
                AuthToken = Guid.NewGuid().ToString(),
                Password = hash
            };

            PlayerDB.Players.Insert(newFullPlayer);

            Console.WriteLine($"[REGISTER] Created account {newFullPlayer.PlayerId}: {newFullPlayer.Player.Username}");

            return Login(newFullPlayer, logAsLogin: true);
        }

        private IActionResult HandleRefreshTokenGrant(IFormCollection form)
        {
            string refreshToken =
                FirstFormValue(form, "refresh_token", "refreshToken").Trim();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return OAuthError("invalid_request", "refresh_token is required.");
            }

            string hash = Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(refreshToken)));

            // For now, just create a new token for any valid refresh token
            // In production, we'd validate against stored tokens
            var player = PlayerDB.Players.FindAll().FirstOrDefault();
            if (player == null || player.Player == null)
            {
                return OAuthError("invalid_grant", "No valid account found.");
            }

            return Login(player, logAsLogin: false);
        }

        private IActionResult Login(PDBC.FullPlayer player, bool logAsLogin)
        {
            if (player.Player == null)
            {
                return OAuthError("invalid_grant", "Account not found.");
            }

            if (logAsLogin)
            {
                player.Player.LastLoginAt = DateTime.UtcNow;
                PlayerDB.Players.Update(player);
            }

            string accessToken = AuthStuff.Encode(player.PlayerId);
            string refreshToken = Convert.ToHexString(
                System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));

            Console.WriteLine($"[AUTH TOKEN ISSUED] account={player.PlayerId} type={(logAsLogin ? "login" : "refresh")}");

            return Ok(new
            {
                access_token = accessToken,
                token_type = "bearer",
                expires_in = 2592000,
                account_id = player.PlayerId,
                error = "",
                error_description = "",
                refresh_token = refreshToken,
                key = ""
            });
        }

        private IActionResult CheckModGate(IFormCollection form)
        {
            string buildHeader = Request.Headers["X-EMBER-Build"].ToString();

            if (string.IsNullOrWhiteSpace(buildHeader))
            {
                // Stock client without Ember patch - allow login but log warning
                Console.WriteLine("[MOD GATE] No X-EMBER-Build header - stock client login allowed");
                return null;
            }

            if (!Version.TryParse(buildHeader, out var build) ||
                build < new Version(0, 0, 1))
            {
                return OutdatedPatch();
            }

            string steamIdHeader = Request.Headers["X-EMBER-SteamId"].ToString();
            bool hasSteamId =
                ulong.TryParse(steamIdHeader, out var steamId) &&
                steamId != 0;

            TryReadPlatform(form, out var platform, out var platformId);
            bool isQuest = platform == PlayerDBClasses.Platforms.Oculus;

            if (!hasSteamId && !isQuest)
            {
                return OutdatedPatch();
            }

            if (hasSteamId && false)
            {
                return SteamIdForbidden(steamId);
            }

            return null;
        }

        private BadRequestObjectResult OutdatedPatch()
        {
            return OAuthError("outdated_client", "You're on the old patch.\r\nUpdate the patch in the Discord :3\r\nThis shows up once.");
        }

        private static bool TryReadPlatform(IFormCollection form, out PlayerDBClasses.Platforms platform, out ulong platformId)
        {
            platform = default;
            platformId = 0;

            string platformText =
                FirstFormValue(form, "platform", "platform_type", "platformType", "platform_name", "platformName");

            if (!TryParsePlatform(platformText, out platform))
            {
                return false;
            }

            return TryReadPlatformId(form, out platformId);
        }

        private static bool TryReadPlatformId(IFormCollection form, out ulong platformId)
        {
            platformId = 0;

            string platformIdText =
                FirstFormValue(form, "platform_id", "platformId", "platformid", "platform_user_id",
                    "platformUserId", "platformuserid", "oculus_user_id", "oculusUserId",
                    "oculus_userid", "oculus_id", "oculusId", "meta_user_id", "metaUserId",
                    "meta_userid", "quest_user_id", "questUserId", "quest_userid",
                    "user_id", "userId");

            return ulong.TryParse(platformIdText, out platformId) && platformId != 0;
        }

        private static bool TryParsePlatform(string platformText, out PlayerDBClasses.Platforms platform)
        {
            platform = default;

            if (string.IsNullOrWhiteSpace(platformText))
            {
                return false;
            }

            platformText = platformText.Trim();

            if (int.TryParse(platformText, out int platformNumber))
            {
                platform = (PlayerDBClasses.Platforms)platformNumber;
                return IsUsablePlatform(platform);
            }

            if (Enum.TryParse(platformText, true, out platform) && IsUsablePlatform(platform))
            {
                return true;
            }

            string normalized = NormalizeEnumText(platformText);

            bool isQuestAlias =
                normalized is "quest" or "quest2" or "quest3" or "questpro" or "meta" or "metaquest" or "oculus" or "oculusquest" or "oculusstandalone";

            if (!isQuestAlias)
            {
                return false;
            }

            foreach (string enumName in Enum.GetNames<PlayerDBClasses.Platforms>())
            {
                string normalizedEnumName = NormalizeEnumText(enumName);

                bool looksLikeQuestPlatform =
                    normalizedEnumName.Contains("quest", StringComparison.Ordinal) ||
                    normalizedEnumName.Contains("oculus", StringComparison.Ordinal) ||
                    normalizedEnumName is "meta" or "metaquest";

                if (!looksLikeQuestPlatform)
                {
                    continue;
                }

                if (Enum.TryParse(enumName, true, out platform) && IsUsablePlatform(platform))
                {
                    return true;
                }
            }

            platform = default;
            return false;
        }

        private static bool IsUsablePlatform(PlayerDBClasses.Platforms platform)
        {
            return Enum.IsDefined(typeof(PlayerDBClasses.Platforms), platform) &&
                   platform is not PlayerDBClasses.Platforms.All and not PlayerDBClasses.Platforms.HeadlessBot;
        }

        private static string NormalizeEnumText(string value)
        {
            return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        }

        private static bool TryReadAccountId(IFormCollection form, out long accountId)
        {
            accountId = 0;

            string accountIdText =
                FirstFormValue(form, "account_id", "accountId", "accountid", "player_id", "playerId");

            return long.TryParse(accountIdText, out accountId) && accountId > 0;
        }

        private static string FirstFormValue(IFormCollection form, params string[] keys)
        {
            foreach (string key in keys)
            {
                string? value = form[key].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
        }

        private IActionResult SteamIdForbidden(ulong steamId)
        {
            Console.WriteLine($"[AUTH STEAM BLOCKED] steamId={steamId}");

            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    error = "steam_id_blacklisted",
                    error_description = "This Steam ID has been blocked from EMBER."
                });
        }

        private BadRequestObjectResult OAuthError(string error, string description)
        {
            return BadRequest(new
            {
                error,
                error_description = description
            });
        }
    }
}
