namespace asdfasdf;

[Serializable]
public class BulkAccount
{
    public int AvailableUsernameChanges { get; set; }
    public string Email { get; set; }
    public DateTime Birthday { get; set; }
    public bool IsFakeJuniorBirthday { get; set; }
    public int accountId { get; set; }
    public string username { get; set; }
    public string displayName { get; set; }
    public string displayEmoji { get; set; }
    public string profileImage { get; set; }
    public string bannerImage { get; set; }
    public bool isJunior { get; set; }
    public PlatformType platforms { get; set; }
    public PersonalPronouns personalPronouns { get; set; }
    public IdentityFlags identityFlags { get; set; }
    public DateTime createdAt { get; set; }
    public bool isMetaPlatformBlocked { get; set; }
}