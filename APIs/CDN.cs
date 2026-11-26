namespace asdfasdf.APIs;

public static class CDN
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/config/1b057e6e-979d-4f30-8856-a386f77c90da", () => File.ReadAllText("wwwroot/config/1b057e6e-979d-4f30-8856-a386f77c90da"));

        app.MapGet("/config/LoadingScreenTipData", () =>
        {
            return File.ReadAllText("LoadingScreenTipData.json");
        });
    }
}