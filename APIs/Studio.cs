namespace asdfasdf.APIs;

public static class Studio
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/analytics/rudderstack-api-key", () =>
        {
            return "23NiJHIgu3koaGNCZIiuYvIQNCu";
        });

        app.MapGet("/access/override/0", () =>
        {
            app.Logger.LogWarning("(Studio) /access/override/0 STUB");

            AccessOverrideDTO access = new()
            {
                AccountId = Accounts.account.accountId,
                AvatarStudioAccess = AvatarStudioAccessFlags.Avatars | AvatarStudioAccessFlags.Roomie | AvatarStudioAccessFlags.Badges | AvatarStudioAccessFlags.Consumables,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                StudioAccess = StudioAccessType.AlwaysAllow
            };
    
            return access;
        });

        app.MapGet("/collaboration/owners-in-room", (int roomId, long subRoomId) =>
        {
            app.Logger.LogWarning("(Studio) /collaboration/owners-in-room STUB");

            StudioOwnersInRoom ownersInRoom = new()
            {
                success = true,
            };

            return ownersInRoom;
        });

        app.MapGet("/cloud-builds/for-room", (int roomId, int subRoomId, int skip, int take) =>
        {
            return new CloudBuildDTO()
            {
                CloudBuildId = string.Empty,
                StartedAt = DateTime.MinValue,
                CompletedAt = DateTime.MinValue,
                Error = string.Empty,
                UnityAssetId = string.Empty,
                CreatedByAccountId = 3
            };
        });








    }
}