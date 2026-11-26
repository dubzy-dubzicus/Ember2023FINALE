using System.Text.Json;

namespace asdfasdf.APIs;

public static class NameServer
{
    public static void Init(WebApplication app)
    {
        if (!File.Exists("ServerData/ns.json"))
        {
            string url = "https://127.0.0.1";
            NameServerResponse ns = new()
            {
                Accounts = url,
                AI = url,
                API = url,
                Auth = url,
                BugReporting = url,
                Cards = url,
                CDN = url,
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
                Images = url + "/img",
                Leaderboard = url,
                Link = url,
                Lists = url,
                Matchmaking = url,
                Moderation = url,
                Notifications = url,
                PlatformNotifications = url,
                PlayerSettings = url,
                RoomComments = url,
                RoomieIntegrations = url,
                Rooms = url,
                Storage = url,
                Strings = url,
                StringsCDN = url,
                Studio = url,
                StudioCDN = url,
                Thorn = url,
                Videos = url,
                WWW = url
            };
            
            File.WriteAllText("ServerData/ns.json", JsonSerializer.Serialize(ns));
        }
        
        app.MapGet("/", () => File.ReadAllText("ServerData/ns.json"));
    }
}