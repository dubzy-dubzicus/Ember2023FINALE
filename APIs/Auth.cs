using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace asdfasdf.APIs;

public static class Auth
{
    public static List<LoggedInUser> currentUsers;
    
    public static void Init(WebApplication app)
    {
        currentUsers = new();

        if (File.Exists("ServerData/LoggedInUsers.json"))
        {
            currentUsers = JsonSerializer.Deserialize<List<LoggedInUser>>(File.ReadAllText("ServerData/LoggedInUsers.json"));
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
                verification_uri = "https://127.0.0.1/other/deviceauth.html",
                verification_uri_complete = "https://127.0.0.1/other/deviceauth.html",
                expires_in = 69420,
                interval = 5
            };

            return Results.Created("/connect/deviceauthorization", response);
        });

        app.MapPost("/connect/token", ([FromForm] string? client_id, [FromForm] string? grant_type, [FromForm] string? platform_id, [FromForm] int? device_class, HttpRequest request) =>
        {
            if (!string.IsNullOrEmpty(client_id) && string.Equals(client_id, "recroom.studio"))
            {
                device_class = 0;
                
                TokenResponse tokenrrs = new()
                {
                    access_token = BuildJWTToken(),
                    expires_in = 43200,
                    key = "FFez0R2bkFhD60xs4h1Vz9IczHop8Y90xGfaQ7wVLGU=",
                    refresh_token = "1093F3F27DA152B7EB51E02D12193FE961B89A4D1953B1817FBC0CDEF5E6F47C",
                    scope = "offline_access rn",
                    token_type = "Bearer"
                };

                CachedAccount accountrrs = JsonSerializer.Deserialize<CachedAccount>(File.ReadAllText("ServerData/CachedAccounts/" + "76561198376170927" + ".json"));
            
                currentUsers.Add(new()
                {
                    token = "Bearer " + tokenrrs.access_token,
                    id = accountrrs.AccountId,
                    account = JsonSerializer.Deserialize<BulkAccount>(File.ReadAllText("ServerData/Accounts/" + accountrrs.AccountId + "/user.json")),
                    outfits = JsonSerializer.Deserialize<List<OutfitSaveRequest>>(File.ReadAllText("ServerData/Accounts/" + accountrrs.AccountId + "/Outfits.json")),
                    ps = JsonSerializer.Deserialize<List<PlayerSetting>>(File.ReadAllText("ServerData/Accounts/" + accountrrs.AccountId + "/PlayerSettings.json")),
                    deviceClass = device_class.Value
                });
            
                return Results.Created("/connect/token", tokenrrs);
            }
            
            if (grant_type.Equals("create_account"))
            {
                var response = new
                {
                    error = "invalid_grant",
                    error_description = "Lemres"
                };

                return Results.BadRequest(response);
            }
            
            TokenResponse token = new()
            {
                access_token = BuildJWTToken(),
                expires_in = 43200,
                key = "FFez0R2bkFhD60xs4h1Vz9IczHop8Y90xGfaQ7wVLGU=",
                refresh_token = "1093F3F27DA152B7EB51E02D12193FE961B89A4D1953B1817FBC0CDEF5E6F47C",
                scope = "offline_access rn",
                token_type = "Bearer"
            };

            CachedAccount account = JsonSerializer.Deserialize<CachedAccount>(File.ReadAllText("ServerData/CachedAccounts/" + platform_id + ".json"));
            
            currentUsers.Add(new()
            {
                token = "Bearer " + token.access_token,
                id = account.AccountId,
                account = JsonSerializer.Deserialize<BulkAccount>(File.ReadAllText("ServerData/Accounts/" + account.AccountId + "/user.json")),
                outfits = JsonSerializer.Deserialize<List<OutfitSaveRequest>>(File.ReadAllText("ServerData/Accounts/" + account.AccountId + "/Outfits.json")),
                ps = JsonSerializer.Deserialize<List<PlayerSetting>>(File.ReadAllText("ServerData/Accounts/" + account.AccountId + "/PlayerSettings.json")),
                deviceClass = device_class.Value
            });
            
            return Results.Created("/connect/token", token);
        }).DisableAntiforgery();

        app.MapGet("/cachedlogin/forplatformid/0/{steamId}", (string steamId) =>
        {
            if (!File.Exists("ServerData/CachedAccounts/" + steamId + ".json"))
            {
                app.Logger.LogInformation("Creating user for " + steamId);

                int accountId = Directory.GetDirectories("ServerData/Accounts").Length + 1;
                
                Accounts.CreateAccount(accountId, steamId);

                CachedAccount cached = new()
                {
                    AccountId = accountId,
                    LastLoginTime = DateTime.UtcNow,
                    Platform = PlatformType.All,
                    PlatformId = steamId,
                    RequirePassword = false
                };
    
                File.WriteAllText("ServerData/CachedAccounts/" + steamId + ".json", JsonSerializer.Serialize(cached));
            }
            
            List<CachedAccount> accounts = new();

            CachedAccount cachedAccount = JsonSerializer.Deserialize<CachedAccount>(File.ReadAllText("ServerData/CachedAccounts/" + steamId + ".json"));
            
            accounts.Add(cachedAccount);
            
            return accounts;
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
            // AccountPrivilege privilege = new()
            // {
            //     AccountId = 0,
            //     Name = "User",
            //     Description = "stub",
            //     EndDate = DateTime.UtcNow,
            //     AssociatedAccountid = 0,
            //     AssociatedAccountUsername = "stub",
            //     ReportCategory = ReportCategory.Moderator,
            //     DisplayReason = "Lemres"
            // };
            //
            // return privilege;
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

            return Results.Ok(new {ImageName = fileName});
        }).DisableAntiforgery();
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