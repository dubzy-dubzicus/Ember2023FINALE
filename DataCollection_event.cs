namespace asdfasdf;

[Serializable]
public class DataCollection_event
{
    public string EventType { get; set; }
    public EventParams EventParams { get; set; }
}

[Serializable]
public class EventParams
{
    public string event_type { get; set; }
    public string app_version { get; set; }
    public string ip { get; set; }
    public string language { get; set; }
    public DateTime event_time { get; set; }
    public DateTime time { get; set; }
    public string session_id { get; set; }
    public int event_id { get; set; }
    public int sequence_number { get; set; }
    public string insert_id { get; set; }
    public string stable_id { get; set; }
    public EventProperties event_properties { get; set; }
    public string _EventType { get; set; }
    public string _Id { get; set; }
    public string _GameSessionId { get; set; }
    public string _AccountId { get; set; }
    public string _Time { get; set; }
    public string _TimenUtc { get; set; }
}

[Serializable]
public class EventProperties
{
    public string game_platform { get; set; }
    public string client_launcher { get; set; }
    public string user_crm { get; set; }
}