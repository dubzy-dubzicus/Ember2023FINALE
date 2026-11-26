namespace asdfasdf;

[Serializable]
public class CloudBuildDTO
{
    public string CloudBuildId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Error { get; set; }
    public string UnityAssetId { get; set; }
    public int CreatedByAccountId { get; set; }
}