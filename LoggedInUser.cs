using System.Text.Json;
using asdfasdf.APIs;

namespace asdfasdf;

[Serializable]
public class LoggedInUser
{
    public string token { get; set; }
    public int id { get; set; }
    public List<OutfitSaveRequest> outfits { get; set; }
    public BulkAccount account { get; set; }
    public int currentStatusVisibility { get; set; }
    public int deviceClass { get; set; }
    public Heartbeat heartbeat { get; set; }
    public RoomInstance currentInstance { get; set; }
    public List<PlayerSetting> ps { get; set; }

    public void UpdateHeartbeat()
    {
        heartbeat = new()
        {
            appVersion = "20230302",
            deviceClass = deviceClass,
            errorCode = 0,
            isOnline = true,
            playerId = id,
            roomInstance = null,
            statusVisibility = currentStatusVisibility,
            vrMovementMode = 0
        };

        if (currentInstance != null)
        {
            heartbeat.roomInstance = currentInstance;
        }
        
        File.WriteAllText("ServerData/LoggedInUsers.json", JsonSerializer.Serialize(Auth.currentUsers));
    }
}