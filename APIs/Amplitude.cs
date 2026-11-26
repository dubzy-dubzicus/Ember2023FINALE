namespace asdfasdf.APIs;

public static class Amplitude
{
    public static void Init(WebApplication app)
    {
        // All of the stuff in amplitude is just to make the Rec Room logs less spammed

        app.MapPost("/httpapi", () =>
        {
            return Results.Ok();
        });

        app.MapPost("/identify", () =>
        {
            return Results.Ok();
        });
    }
}