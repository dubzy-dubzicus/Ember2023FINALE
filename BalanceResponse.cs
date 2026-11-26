namespace asdfasdf;

[Serializable]
public class BalanceResponse
{
    public long Balance { get; set; }
    public int CurrencyType { get; set; }
    public StorefrontBalanceType BalanceType { get; set; }
}

public enum CurrencyType
{
    Invalid = 0,
    LaserTagTickets = 1,
    RecCenterTokens = 2,
    LostSkullsGold = 3,
    DraculaSilver = 4,
    RecRoyale_Season1 = 5,
    RoomCurrency = 6,
    RoomInventoryItem = 7,
    ProgressionEvent = 8,
    RoomieCredits = 9,
    PaintballPaint = 10,
}

public enum StorefrontBalanceType
{
    NonPurchasedNotUsableInP2P = 0,
    NonPurchasedDefault = 1,
    SteamPurchased = 2,
    OculusPurchased = 3,
    PlayStationPurchased = 4,
    MicrosoftPurchased = 5,
    RecNetPurchased = 6,
    IOSPurchased = 7,
    GooglePlayPurchased = 8,
    PicoPurchased = 9,
    SwitchPurchased = 10,
    PlayStationNonPurchasedP2P = 11,
    NonPlayStationNonPurchasedP2P = 12,
    NonPurchasedEarnedByP2P = 13,
    NonPurchasedEarnedByP2Pv2 = 14,
    UndistributedRoomEarnings = 15,
    RoomieResetEveryRRPlusMonth = 16,
    RoomieEnergyPack = 17,
    RoomieVideoContest = 18,
    GameAIOverspend = 19,
    GameAIDeveloperSpend = 20,
    GameAIPromo = 21,
    GameAIPromoPrivateRoom = 22,
}