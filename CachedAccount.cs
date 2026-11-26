namespace asdfasdf;

[Serializable]
public class CachedAccount
{
    public int AccountId { get; set; }
    public DateTime LastLoginTime { get; set; }
    public PlatformType Platform { get; set; }
    public string PlatformId { get; set; }
    public bool RequirePassword { get; set; }
}