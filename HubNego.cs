namespace asdfasdf;

[Serializable]
public class HubNego
{
    public string negotiateVersion { get; set; }
    public string connectionId { get; set; }
    public string connectionToken { get; set; }
    public AvailableTransports[] availableTransports { get; set; }
}

public class AvailableTransports
{
    public string transport { get; set; }
    public string[] transferFormats { get; set; }
}

