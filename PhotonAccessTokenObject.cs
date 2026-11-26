namespace asdfasdf;

[Serializable]
public class PhotonAccessTokenObject
{
    public PhotonPermission[] Permissions { get; set; }
    public string PhotonAccessToken { get; set; }
    public long RoomInstanceId { get; set; }
}