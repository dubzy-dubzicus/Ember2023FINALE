namespace asdfasdf.APIs;

public static class PlatformNotifications
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/crm/me/config/v3", () =>
        {
            return File.ReadAllText("PlatformNotificationsConfigV3.json");
        });
    }
}