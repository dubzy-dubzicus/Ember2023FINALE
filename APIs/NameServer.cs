using System.Text.Json;

namespace asdfasdf.APIs;

public static class NameServer
{
    // Public entrypoint is Tailscale Funnel only. The backend itself stays on
    // http://127.0.0.1:2059; clients must never be told to call that directly.
    private const string PublicUrl = "https://ember.tail298318.ts.net";

    public static void Init(WebApplication app)
    {
        const string path = "ServerData/ns.json";

        if (!File.Exists(path))
        {
            NameServerResponse ns = new()
            {
                Accounts = PublicUrl,
                AI = PublicUrl,
                API = PublicUrl,
                Auth = PublicUrl,
                BugReporting = PublicUrl,
                Cards = PublicUrl,
                CDN = PublicUrl,
                Chat = PublicUrl,
                Clubs = PublicUrl,
                CMS = PublicUrl,
                Commerce = PublicUrl,
                Data = PublicUrl,
                DataCollection = PublicUrl,
                Discovery = PublicUrl,
                Econ = PublicUrl,
                GameLogs = PublicUrl,
                Geo = PublicUrl,
                Images = PublicUrl + "/img",
                Leaderboard = PublicUrl,
                Link = PublicUrl,
                Lists = PublicUrl,
                Matchmaking = PublicUrl,
                Moderation = PublicUrl,
                Notifications = PublicUrl,
                PlatformNotifications = PublicUrl,
                PlayerSettings = PublicUrl,
                RoomComments = PublicUrl,
                RoomieIntegrations = PublicUrl,
                Rooms = PublicUrl,
                Storage = PublicUrl,
                Strings = PublicUrl,
                StringsCDN = PublicUrl,
                Studio = PublicUrl,
                StudioCDN = PublicUrl,
                Thorn = PublicUrl,
                Videos = PublicUrl,
                WWW = PublicUrl
            };

            File.WriteAllText(path, JsonSerializer.Serialize(ns));
        }
        else
        {
            // Repair older generated files that still point clients at loopback.
            string current = File.ReadAllText(path);
            string repaired = current
                .Replace("http://127.0.0.1", PublicUrl)
                .Replace("https://127.0.0.1", PublicUrl);

            if (!string.Equals(current, repaired, StringComparison.Ordinal))
            {
                File.WriteAllText(path, repaired);
            }
        }

        app.MapGet("/", () => File.ReadAllText(path));
    }
}
