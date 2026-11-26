namespace asdfasdf;

[Serializable]
public class ModerationBlockDetail
{
    public ReportCategory ReportCategory { get; set; }
    public int Duration { get; set; }
    public long GameSessionId { get; set; }
    public bool IsHostKick { get; set; }
    public string Message { get; set; }
    public Int32 PlayerIdReporter { get; set; }
    public bool IsBan { get; set; }
    public bool IsVoiceModAutoban { get; set; }
    public bool IsDeviceBan { get; set; }
    public bool IsWarning { get; set; }
    public string VoteKickReason { get; set; }
    public DateTime TimeoutStartedAt { get; set; }
    public string AssociatedAccountUsername { get; set; }
    public bool ShowCreatorCodeOfConduct { get; set; }
    public string TopMessageOverride { get; set; }
    public string BottomMessageOverride { get; set; }
    public float BlockStartTime { get; set; }
}

public enum ReportCategory
{
    Moderator = 0,
    Unknown = 1,
    DEPRECATED_MicrophoneAbuse = 2,
    Harassment = 3,
    Cheating = 4,
    DEPRECATED_ImmatureBehavior = 5,
    AFK = 6,
    Misc = 7,
    Underage = 8,
    VoteKick = 9,
    MisleadingPurchases = 10,
    CoC_Underage = 11,
    CoC_Sexual = 12,
    CoC_Discrimination = 13,
    CoC_Trolling = 14,
    CoC_NameOrProfile = 15,
    InappropriateClothing = 16,
    IssuingInaccurateReports = 17,
    RoomInventoryItems = 18,
    InappropriateRooms = 19,
    InappropriateInventions = 20,
    RoomOffers = 21,
    Spam = 22
}