namespace asdfasdf.APIs;

public static class Moderation
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/voice/config", () =>
        {
            var result = new
            {
                enabled = false
            };
            return result;
        });
    }
}