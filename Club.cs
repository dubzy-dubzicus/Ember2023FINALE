namespace asdfasdf;

[Serializable]
public class Club
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string MainImageName { get; set; }
    public ClubState State { get; set; }
    public string PrimaryTag { get; set; }
    public ClubVisibility Visibility { get; set; }
    public ClubJoinability Joinability { get; set; }
    public bool AllowJuniors { get; set; }
    public int MinLevel { get; set; }
    public int MemberCount { get; set; }
    public Int64 ClubhouseRoomId { get; set; }
    public ClubType ClubType { get; set; }
}

public enum ClubState
{
    Active = 0,
    PendingJunior = 1,
    Moderation_PendingReview = 2,
    Moderation_Closed = 3,
    MarkedForDelete = 4,
}

public enum ClubVisibility
{
    Private = 0,
    Public = 1,
}

public enum ClubJoinability
{
    Open = 0,
    InviteOnly = 1,
    AskToJoin = 2,
}

public enum ClubType
{
    Generic = 0,
    Creator = 1,
}