namespace asdfasdf.APIs;

public static class DataCollection
{
    public static void Init(WebApplication app)
    {
        app.MapPost("/data/event", (HttpRequest request) =>
        {
            return Results.Created("/data/event", "");
        });

        app.MapPost("/data/heartbeat", () =>
        {
            return Results.Created("/data/heartbeat", "");
        });
    }
}