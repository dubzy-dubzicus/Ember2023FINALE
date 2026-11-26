namespace asdfasdf;

[Serializable]
public class RoomInstance
{
    public bool EncryptVoiceChat { get; set; }
    public int clubId { get; set; }
    public int eventId { get; set; }
    public bool isFull { get; set; }
    public bool isInProgress { get; set; }
    public bool isPrivate { get; set; }
    public string location { get; set; }
    public int matchmakingPolicy { get; set; }
    public int maxCapacity { get; set; }
    public string name { get; set; }
    public string photonRegion { get; set; }
    public string photonRegionId { get; set; }
    public string photonRoomId { get; set; }
    public string roomCode { get; set; }
    public int roomId { get; set; }
    public long roomInstanceId { get; set; }
    public int roomInstanceType { get; set; }
    public long subRoomId { get; set; }
}

