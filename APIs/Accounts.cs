using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace asdfasdf.APIs;

public static class Accounts
{
    public static BulkAccount account = JsonSerializer.Deserialize<BulkAccount>(File.ReadAllText("ServerData/Accounts/2/user.json"));

    public static void CreateAccount(int accountId, string name)
    {
        if (!Directory.Exists("ServerData/Accounts/" + accountId))
        {
            Directory.CreateDirectory("ServerData/Accounts/" + accountId);
        }
        
        BulkAccount tAccount = new()
        {
            AvailableUsernameChanges = 1,
            Email = "email@email.com",
            Birthday = DateTime.MinValue,
            IsFakeJuniorBirthday = false,
            accountId = accountId,
            username = name,
            displayName = name,
            displayEmoji = "",
            profileImage = "DefaultProfileImage.png",
            bannerImage = "DefaultProfileImage.png",
            isJunior = false,
            platforms = PlatformType.All,
            personalPronouns = PersonalPronouns.None,
            identityFlags = IdentityFlags.None,
            createdAt = DateTime.UtcNow,
            isMetaPlatformBlocked = false
        };
        
        File.WriteAllText("ServerData/Accounts/" + accountId + "/user.json", JsonSerializer.Serialize(tAccount));
        File.WriteAllText("ServerData/Accounts/" + accountId + "/Outfits.json", "[]");
        File.Copy("DefaultPlayerSettings.json", "ServerData/Accounts/" + accountId + "/PlayerSettings.json");
        File.WriteAllText("ServerData/Accounts/" + accountId + "/avoidjuniors", "false");
        File.Copy("DefaultAvatar.json", "ServerData/Accounts/" + accountId + "/Avatar.json");
    }
    
    public static void Init(WebApplication app)
    {
        app.MapGet("/account/me", (HttpRequest request) =>
        {
            app.Logger.LogInformation(request.Headers.Authorization[0]);

            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    return Results.Ok(user.account);
                }
            }
            
            return Results.NotFound();
        });
        
        app.MapPut("/account/me/displayname", ([FromForm] string displayName, HttpRequest request) =>
        {
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    user.account.displayName = displayName;
                    File.WriteAllText("ServerData/Accounts/" + user.id + "/user.json", JsonSerializer.Serialize(user.account));

                    ApiResponse response = new()
                    {
                        success = true,
                        error = string.Empty
                    };
                    
                    return Results.Ok(response);
                }
            }
            
            return Results.NotFound();
        }).DisableAntiforgery();

        app.MapPut("/account/me/profileimage", ([FromForm] string imageName, HttpRequest request) =>
        {
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    user.account.profileImage = imageName;
                    File.WriteAllText("ServerData/Accounts/" + user.id + "/user.json", JsonSerializer.Serialize(user.account));

                    ApiResponse response = new()
                    {
                        success = true,
                        error = string.Empty
                    };
                    
                    return Results.Ok(response);
                }
            }
            
            return Results.NotFound();
        }).DisableAntiforgery();

        app.MapPut("/account/me/username", ([FromForm] string username, HttpRequest request) =>
        {
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    user.account.username = username;
                    File.WriteAllText("ServerData/Accounts/" + user.id + "/user.json", JsonSerializer.Serialize(user.account));

                    ApiResponse response = new()
                    {
                        success = true,
                        error = string.Empty
                    };
                    
                    return Results.Ok(response);
                }
            }
            
            return Results.NotFound();
        }).DisableAntiforgery();

        app.MapGet("/account/bulk", (int id) =>
        {
            List<BulkAccount> accounts = new();
    
            accounts.Add(JsonSerializer.Deserialize<BulkAccount>(File.ReadAllText("ServerData/Accounts/" + id + "/user.json")));
    
            return accounts;
        });

        app.MapGet("/role/developer/{id}", () =>
        {
            return true;
        });

        app.MapGet("/parentalcontrol/me", () =>
        {
            ParentalControl pc = new()
            {
                accountId = account.accountId,
                disallowInAppPurchases = false
            };
    
            return pc;
        });

        app.MapGet("/accountprivacysettings/{id}", () =>
        {
            app.Logger.LogWarning("(Accounts) /accountprivacysettings/{{id}} STUB");

            PrivacySettings privacySettings = new()
            {
                accountId = account.accountId,
                isRecentHistoryVisible = true
            };

            return privacySettings;
        });
    }
}