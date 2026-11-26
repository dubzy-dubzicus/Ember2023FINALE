using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace asdfasdf.APIs;

public static class PlayerSettingsAPI
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/playersettings", (HttpRequest request) =>
        {
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    return user.ps;
                }
            }

            return null;
        });

        app.MapPut("/playersettings", ([FromForm] string key, [FromForm] string value, HttpRequest request) =>
        {
            List<PlayerSetting> ps = null;
            int id = 0;
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    ps = user.ps;
                    id = user.id;
                }
            }
            
            if (ps.Exists(x => x.Key == key))
            {
                app.Logger.LogInformation("Removing setting " + key);
                ps.Remove(ps.Find(x => x.Key == key));
            }
    
            app.Logger.LogInformation("Adding setting " + key + " with value " + value);
    
            ps.Add(new(){Key = key, Value = value});
    
            File.WriteAllText("ServerData/Accounts/" + id + "/PlayerSettings.json", JsonSerializer.Serialize(ps));
    
            ApiResponseNoValue response = new()
            {
                success = true
            };

            return response;
        }).DisableAntiforgery();
    }
}