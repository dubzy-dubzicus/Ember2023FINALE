namespace asdfasdf.APIs;

public static class Clubs
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/club/home/me", () =>
        {
            app.Logger.LogWarning("(Clubs) /club/home/me STUB");

            return Results.NotFound();

            // Club club = new()
            // {
            //     Name = "A club",
            //     Description = "A club description",
            //     MainImageName = "DefaultProfileImage.png",
            //     State = ClubState.Active,
            //     PrimaryTag = "Hangout",
            //     Visibility = ClubVisibility.Public,
            //     Joinability = ClubJoinability.Open,
            //     AllowJuniors = true,
            //     MinLevel = 1,
            //     MemberCount = 1,
            //     ClubhouseRoomId = 0,
            //     ClubType = ClubType.Generic
            // };
            //
            // return club;
        });

        app.MapGet("/announcements/v2/mine/unread", () =>
        {
            app.Logger.LogWarning("(Clubs) /announcements/v2/mine/unread STUB");

            return "[]";
        });

        app.MapGet("/club/mine/member", () =>
        {
            app.Logger.LogWarning("(Clubs) /club/mine/member STUB");

            return "[]";
        });

        app.MapGet("/announcements/v2/subscription/mine/unread", () =>
        {
            app.Logger.LogWarning("(Clubs) /announcements/v2/subscription/mine/unread STUB");

            return "[]";
        });

        app.MapGet("/subscription/mine/member", () =>
        {
            app.Logger.LogWarning("(Clubs) /subscription/mine/member STUB");

            return "[]";
        });

        app.MapGet("/subscription/details/{id}", () =>
        {
            app.Logger.LogWarning("(Clubs) /subscription/details/{{id}} STUB");

            return "{}";
        });
    }
}