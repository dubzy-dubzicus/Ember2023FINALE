namespace asdfasdf.APIs;

public static class Data
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/commonQuery/rrstudio/chiprecommendations", () =>
        {
            return File.ReadAllText("studio_chiprecommendations.json");
        });
    }
}