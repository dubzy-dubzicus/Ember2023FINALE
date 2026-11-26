namespace asdfasdf;

[Serializable]
public class CampusCardSubscription
{
    public bool canBuySubscription { get; set; }
    public RRPlusSubscription subscription { get; set; }
    public int platformAccountSubscribedPlayerId { get; set; }
}

[Serializable]
public class RRPlusSubscription
{
    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsAutoRenewing { get; set; }
    public int Level { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int Period { get; set; }
    public string PlatformId { get; set; }
    public string PlatformPurchaseId { get; set; }
    public int PlatformType { get; set; }
    public int RecNetPlayerId { get; set; }
    public int SubscriptionId { get; set; }
}