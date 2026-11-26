namespace asdfasdf;

[Serializable]
public class RoomPublishStateConfig
{
    public int UpdateMaxCount { get; set; }
    public int UpdateRollingWindowInDays { get; set; }
    public int UpdateExpirationInDays { get; set; }
    public int UpdateCooldownInDays { get; set; }
}