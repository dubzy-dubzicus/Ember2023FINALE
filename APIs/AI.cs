namespace asdfasdf.APIs;

public static class AI
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/roomieai/user/access", () =>
        {
            RoomieUserAccess userAccess = new()
            {
                error = "All purchased Roomie energy has been used",
                error_id = "AI.InsufficientRoomieAICreditNoSub",
                success = false
            };

            return userAccess;
        });
    }
}