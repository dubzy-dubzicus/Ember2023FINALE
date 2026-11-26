namespace asdfasdf.APIs;

public static class Chat
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/thread", (int maxCount, int mode) =>
        {
            app.Logger.LogWarning("(Chat) /thread STUB");

            return "[]";
        });

        app.MapGet("/thread/party", (int maxCount, int mode) =>
        {
            app.Logger.LogWarning("(Chat) /thread/party STUB");

            return "[]";
        });
    }
}