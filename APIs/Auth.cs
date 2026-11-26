using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace asdfasdf.APIs;

public static class Auth
{
    private const string PublicUrl = "https://ember.tail298318.ts.net";
    private const string TokenLocation = "/auth/connect/token";
    private static readonly object AccountLock = new();

    public static List<LoggedInUser> currentUsers;

    public static void Init(WebApplication app)
    {
        currentUsers = new();

        if (File.Exists("ServerData/LoggedInUsers.json"))
        {
            currentUsers = JsonSerializer.Deserialize<List<LoggedInUser>>(File.ReadAllText("ServerData/LoggedInUsers.json")) ?? new();
        }

        app.MapPost("/connect/deviceauthorization", async (HttpRequest request) =>
        {
            var a = await request.ReadFormAsync();
            var clientId = a.ElementAt(0);
            var clientSecret = a.ElementAt(1);

            app.Logger.LogWarning($"(Auth) /connect/deviceauthorization STUB\n{clientId}\n{clientSecret}");

            DeviceAuthorizationResponse response = new()
            {
                device_code = "stub",
                user_code = "stub",
                verification_uri = PublicUrl + "/other/deviceauth.html",
                verification_uri_complete = PublicUrl + "/other/deviceauth.html",
                expires_in = 69420,
                interval = 5
            };

            return Results.Created("/connect/deviceauthorization", response);
        });

        IResult HandleToken(
            [FromForm] string? client_id,
            [FromForm] string? grant_type,
            [FromForm] string? platform_id,
            [FromForm] string? username,
            [FromForm] string? password,
            [FromForm] int? device_class,
            HttpRequest request)
        {
            device_class ??= 0;
            grant_type ??= string.Empty;

            if (string.Equals(client_id, "recroom.studio", StringComparison.OrdinalIgnoreCase))
            {
                device_class = 0;

                TokenResponse tokenrrs = BuildTokenResponse();
                CachedAccount accountrrs = EnsureCachedAccount("76561198376170927");
                AddLoggedInUser(tokenrrs.access_token, accountrrs, device_class.Value);

                return Results.Created(TokenLocation, tokenrrs);
            }

            if (string.Equals(grant_type, "create_account", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest(new
                {
                    error = "invalid_grant",
                    error_description = "Lemres"
                });
            }

            // The patched 2023 client sends grant_type=password plus the Steam
            // ID in X-EMBER-SteamId. Older clients send platform_id instead.
            string? platformId = platform_id;
            if (string.IsNullOrWhiteSpace(platformId) &&
                string.Equals(grant_type, "password", StringComparison.OrdinalIgnoreCase))
            {
                platformId = request.Headers["X-EMBER-SteamId"].ToString();
            }

            if (string.IsNullOrWhiteSpace(platformId))
            {
                platformId = username;
            }

            if (string.IsNullOrWhiteSpace(platformId))
            {
                return Results.BadRequest(new
                {
                    error = "invalid_request",
                    error_description = "platform_id or X-EMBER-SteamId is required"
                });
            }

            app.Logger.LogInformation($"(Auth) token grant={grant_type} client={client_id ?? ""} platform={platformId}");

            TokenResponse token = BuildTokenResponse();
            CachedAccount account = EnsureCachedAccount(platformId);
            AddLoggedInUser(token.access_token, account, device_class.Value);

            return Results.Created(TokenLocation, token);
        }

        // Canonical route used by the client patch, plus the original route for
        // anything already cached against it.
        app.MapPost("/auth/connect/token", HandleToken).DisableAntiforgery();
        app.MapPost("/connect/token", HandleToken).DisableAntiforgery();

        app.MapGet("/cachedlogin/forplatformid/0/{steamId}", (string steamId, HttpRequest request) =>
        {
            CachedAccount cachedAccount = EnsureCachedAccount(steamId);
            return new List<CachedAccount> { cachedAccount };
        });

        app.MapPost("/cachedlogin/forplatformids", () =>
        {
            app.Logger.LogWarning("(Auth) /cachedlogin/forplatformids STUB");

            return "[]";
        });

        app.MapGet("privileges/me/restrictions", () =>
        {
            app.Logger.LogWarning("(Auth) /privileges/me/restrictions STUB");

            return "[]";
        });

        app.MapGet("/eac/challenge", () =>
        {
            return "\"AQAAAOmBNw8TMlSZs9NsuyGxsU7WMoKVcieTW8IyoSMEGcbTIPg7DZ4JGZTCfg4b1h58FOOkM7r0LUhRVwlq8wo528o3LR9g/kHzlrVJA6H0+HaYTQzjKEe+zl5/VupqSXk8PXQ0BlNiRED3x5YMCGiM+F0fYmd0NU4K0d5hY+A=\"";
        });

        app.MapPost("/account/recoverpassword", () =>
        {
            app.Logger.LogWarning("(Auth) /account/recoverpassword STUB");

            ApiResponseNoValue response = new()
            {
                error = "Lemres",
                error_id = "id",
                success = false
            };

            return Results.Created("/account/recoverpassword", response);
        });

        // Legacy?
        app.MapGet("/api/settings/v2", () =>
        {
            app.Logger.LogWarning("(API) /api/settings/v2 STUB");

            return File.ReadAllText("DefaultPlayerSettings.json");
        });

        // Legacy?
        app.MapPost("/api/settings/v2/set", () =>
        {
            app.Logger.LogWarning("(API) /api/settings/v2/set STUB");

            return File.ReadAllText("DefaultPlayerSettings.json");
        });

        app.MapPost("/api/images/v4/uploadsaved", async (HttpRequest request) =>
        {
            var file = request.Form.Files.FirstOrDefault();

            string fileName = Guid.NewGuid() + ".jpg";

            using (var stream = new FileStream("wwwroot/" + fileName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Results.Ok(new { ImageName = fileName });
        }).DisableAntiforgery();
    }

    private static TokenResponse BuildTokenResponse()
    {
        return new TokenResponse
        {
            access_token = BuildJWTToken(),
            expires_in = 43200,
            key = "FFez0R2bkFhD60xs4h1Vz9IczHop8Y90xGfaQ7wVLGU=",
            refresh_token = "1093F3F27DA152B7EB51E02D12193FE961B89A4D1953B1817FBC0CDEF5E6F47C",
            scope = "offline_access rn",
            token_type = "Bearer"
        };
    }

    private static void AddLoggedInUser(string token, CachedAccount cached, int deviceClass)
    {
        BulkAccount account = EnsureAccountFiles(cached.AccountId, cached.PlatformId);

        List<OutfitSaveRequest> outfits = JsonSerializer.Deserialize<List<OutfitSaveRequest>>(
            File.ReadAllText("ServerData/Accounts/" + cached.AccountId + "/Outfits.json")) ?? new();

        List<PlayerSetting> ps = JsonSerializer.Deserialize<List<PlayerSetting>>(
            File.ReadAllText("ServerData/Accounts/" + cached.AccountId + "/PlayerSettings.json")) ?? new();

        currentUsers.Add(new()
        {
            token = "Bearer " + token,
            id = cached.AccountId,
            account = account,
            outfits = outfits,
            ps = ps,
            deviceClass = deviceClass
        });
    }

    private static CachedAccount EnsureCachedAccount(string platformId)
    {
        lock (AccountLock)
        {
            Directory.CreateDirectory("ServerData/CachedAccounts");
            Directory.CreateDirectory("ServerData/Accounts");

            string path = "ServerData/CachedAccounts/" + platformId + ".json";

            if (!File.Exists(path))
            {
                int accountId = Directory.GetDirectories("ServerData/Accounts").Length + 1;
                while (Directory.Exists("ServerData/Accounts/" + accountId))
                {
                    accountId++;
                }

                Accounts.CreateAccount(accountId, platformId);

                CachedAccount created = new()
                {
                    AccountId = accountId,
                    LastLoginTime = DateTime.UtcNow,
                    Platform = PlatformType.All,
                    PlatformId = platformId,
                    RequirePassword = false
                };

                File.WriteAllText(path, JsonSerializer.Serialize(created));
                return created;
            }

            CachedAccount cached = JsonSerializer.Deserialize<CachedAccount>(File.ReadAllText(path))
                ?? throw new InvalidOperationException("Cached account file is empty: " + path);

            EnsureAccountFiles(cached.AccountId, cached.PlatformId ?? platformId);
            return cached;
        }
    }

    private static BulkAccount EnsureAccountFiles(int accountId, string platformId)
    {
        string dir = "ServerData/Accounts/" + accountId;
        Directory.CreateDirectory(dir);

        string userPath = dir + "/user.json";
        if (!File.Exists(userPath))
        {
            Accounts.CreateAccount(accountId, platformId);
        }

        if (!File.Exists(dir + "/Outfits.json"))
        {
            File.WriteAllText(dir + "/Outfits.json", "[]");
        }

        if (!File.Exists(dir + "/PlayerSettings.json"))
        {
            File.Copy("DefaultPlayerSettings.json", dir + "/PlayerSettings.json");
        }

        if (!File.Exists(dir + "/Avatar.json"))
        {
            File.Copy("DefaultAvatar.json", dir + "/Avatar.json");
        }

        if (!File.Exists(dir + "/avoidjuniors"))
        {
            File.WriteAllText(dir + "/avoidjuniors", "false");
        }

        return JsonSerializer.Deserialize<BulkAccount>(File.ReadAllText(userPath))
            ?? throw new InvalidOperationException("Account file is empty: " + userPath);
    }

    private static string BuildJWTToken()
    {
        var roles = new List<string>
        {
            "influencer",
            "developer",
            "moderator",
            "screenshare",
            "keepsake",
            "livekeepsakeeventroomsaveoverride",
            "multiinstanceevent",
            "betastudio",
            "ugcloc"
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKeywqewqeqqqqqqqqqqqweeeeeeeeeeeeeeeeeeeqweqe"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var issuer = "reccc";

        var claims = new[]
        {
            new Claim("sub", "0"),
            new Claim("role", JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var jwtValidity = DateTime.Now.AddDays(30);

        var token = new JwtSecurityToken(issuer,
            claims: claims,
            expires: jwtValidity,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
