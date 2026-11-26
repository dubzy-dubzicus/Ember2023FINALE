using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace asdfasdf;

public class Room
{
    public int RoomId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageName { get; set; }
    public WarningMask WarningMask { get; set; }
    public string CustomWarning { get; set; }
    public int CreatorAccountId { get; set; }
    public int State { get; set; }
    public Accessibility Accessibility { get; set; }
    public int PublishState { get; set; }
    public bool SupportsLevelVoting { get; set; }
    public bool IsRRO { get; set; }
    public bool IsRecRoomApproved { get; set; }
    public bool ExcludeFromLists { get; set; }
    public bool ExcludeFromSearch { get; set; }
    public bool SupportsScreens { get; set; }
    public bool SupportsWalkVR { get; set; }
    public bool SupportsTeleportVR { get; set; }
    public bool SupportsVRLow { get; set; }
    public bool SupportsQuest2 { get; set; }
    public bool SupportsMobile { get; set; }
    public bool SupportsJuniors { get; set; }
    public int MinLevel { get; set; }
    public AgeRating AgeRating { get; set; }
    public string CreatedAt { get; set; }
    public string PublishedAt { get; set; }
    public object BecameRRStudioRoomAt { get; set; }
    public Stats Stats { get; set; }
    public object RankingContext { get; set; }
    public bool IsDorm { get; set; }
    public bool IsPlacePlay { get; set; }
    public int MaxPlayerCalculationMode { get; set; }
    public int MaxPlayers { get; set; }
    public bool CloningAllowed { get; set; }
    public bool DisableMicAutoMute { get; set; }
    public bool DisableRoomComments { get; set; }
    public bool EncryptVoiceChat { get; set; }
    public bool ToxmodEnabled { get; set; }
    public bool LoadScreenLocked { get; set; }
    public int UgcVersion { get; set; }
    public int PersistenceVersion { get; set; }
    public object UgcSubVersion { get; set; }
    public object MinUgcSubVersion { get; set; }
    public bool AutoLocalizeRoom { get; set; }
    public LocalizationContext LocalizationContext { get; set; }
    public bool IsDeveloperOwned { get; set; }
    public string RankedEntityId { get; set; }
    public SubRooms[] SubRooms { get; set; }
    public Roles[] Roles { get; set; }
    public bool IsJuniorCreated { get; set; }
    public List<Tags> Tags { get; set; }
    public object[] PromoImages { get; set; }
    public object[] PromoExternalContent { get; set; }
    public LoadScreen[] LoadScreens { get; set; }
    public object[] RestrictedCircuitsAllowListNames { get; set; }
}

public class Stats
{
    public int CheerCount { get; set; }
    public int FavoriteCount { get; set; }
    public int VisitorCount { get; set; }
    public int VisitCount { get; set; }
}

public class LocalizationContext
{
    public object TargetLocale { get; set; }
    public object Scope { get; set; }
    public string[] LocalizedFields { get; set; }
}

public class SubRooms
{
    public long SubRoomId { get; set; }
    public int RoomId { get; set; }
    public int CreatorAccountId { get; set; }
    public string UnitySceneId { get; set; }
    public string Name { get; set; }
    public CurrentSave CurrentSave { get; set; }
    public int LastModeratedSaveModerationState { get; set; }
    public bool IsSandbox { get; set; }
    public int MaxPlayers { get; set; }
    public int Accessibility { get; set; }
    public bool ShouldAutoStageSaves { get; set; }
    public object StagedSubRoomDataSaveId { get; set; }
}

public class Roles
{
    public int AccountId { get; set; }
    public int Role { get; set; }
    public object LastChangedByAccountId { get; set; }
    public int InvitedRole { get; set; }
}

public class Tags(string tag, int type)
{
    public string Tag { get; set; } = tag;
    public int Type { get; set; } = type;
}

public class RootObject
{
    public CurrentSave CurrentSave { get; set; }
}

public class CurrentSave
{
    public int SubRoomDataSaveId { get; set; }
    public int SubRoomId { get; set; }
    public object UnityAssetId { get; set; }
    public object[] ReferencedUnityAssetIds { get; set; }
    public string DataBlob { get; set; }
    public object DataBlobHash { get; set; }
    public int PersistenceVersion { get; set; }
    public int OMVersion { get; set; }
    public object SavedByAccountId { get; set; }
    public object SavedOnPlatform { get; set; }
    public object SavedOnDeviceClass { get; set; }
    public object Description { get; set; }
    public int ModerationState { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UgcSubVersion { get; set; }
}

public class SaveRequest
{
    public object UnityAssetId { get; set; }
    public RoomData RoomData { get; set; }
    public RoomData SubRoomData { get; set; }
    public string InventionUsage { get; set; }
    public int PersistenceVersion { get; set; }
    public string Description { get; set; }
    public bool AutoPublish { get; set; }
}

public class RoomData
{
    public string Filename { get; set; }
    public string Hash { get; set; }
    public object OwnershipProof { get; set; }
}

public class LoadScreen
{
    public string ImageName { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
}