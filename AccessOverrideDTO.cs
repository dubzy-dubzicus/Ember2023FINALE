namespace asdfasdf;

[Serializable]
public class AccessOverrideDTO
{
    public int AccountId { get; set; }
    public StudioAccessType StudioAccess { get; set; }
    public AvatarStudioAccessFlags AvatarStudioAccess { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

public enum StudioAccessType : byte
{
    StatBased,
    AlwaysAllow,
    NoStudioAccess
}

[Flags]
public enum AvatarStudioAccessFlags
{
    None = 0,
    Avatars = 1,
    Roomie = 2,
    Badges = 4,
    Consumables = 8
}