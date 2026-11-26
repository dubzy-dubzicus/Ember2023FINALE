namespace asdfasdf;

[Serializable]
public class BulkCustomAvatarItem
{
    public string CustomAvatarItemId { get; set; }
    public int CreatorAccountId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public int Accessibility { get; set; }
    public bool ForceCannotPublish { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsRecRoomApproved { get; set; }
    public string BaseAvatarItemId { get; set; }
    public string BaseAvatarItemColor { get; set; }
    public string DesignFilename { get; set; }
    public string ThumbnailImageFilename { get; set; }
    public string CreatedAt { get; set; }
    public string ModifiedAt { get; set; }
    public int PreviewOrientation { get; set; }
    public string RankingContext { get; set; }
    public int OutfitType { get; set; }
    public Array CurrentSaves { get; set; }
    public string PurchaseInfo { get; set; }
    public Array Tags { get; set; }
    public string CustomBadgeMetadata { get; set; }
    public string RankedEntityId { get; set; }
}