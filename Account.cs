namespace asdfasdf;

public class Account
{
    public int AccountId { get; set; }
    public string RawUsername { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string DisplayEmoji { get; set; }
    public string ProfileImage { get; set; }
    public string BannerImage { get; set; }
    public bool TreatAsJunior { get; set; }
    public bool HasBirthday { get; set; }
    public PersonalPronouns PersonalPronouns { get; set; }
    public IdentityFlags IdentityFlags { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsJunior { get; set; }
}