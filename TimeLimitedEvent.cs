namespace asdfasdf;

[Serializable]
public class TimeLimitedEvent
{
    public long TimeLimitedEventId { get; set; }
    public Guid ReplicationId { get; set; }
    public string Name { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Metadata { get; set; }
    public TimeLimitedEventConfig Config { get; set; }
}

[Serializable]
public class TimeLimitedEventConfig
{
    public TimedEventCurrencyGrant CurrencyGrant { get; set; }
    public List<TimedEventXpSink> XpSinks { get; set; }
    public TimeLimitedEventTeamConfigStorage TeamConfig { get; set; }
}

[Serializable]
public class TimedEventCurrencyGrant
{
    public Int64 TimedCurrencyGrantId { get; set; }
}

[Serializable]
public class TimedEventXpSink
{
    public List<TimedEventXpSource> XpSources { get; set; }
    public List<TimedEventXpMultiplier> XpMultipliers { get; set; }
    public string GiftMessage { get; set; }
}

[Serializable]
public class TimedEventXpSource
{
    
}

public class TimedEventXpMultiplier
{
    public float XpMultiplier { get; set; }
}

[Serializable]
public class TimeLimitedEventTeamConfigStorage
{
    public Int64 TimeLimitedEventTeamConfigId { get; set; }
    public TimeLimitedEventTeamConfig TimeLimitedEventTeamConfig { get; set; }
}

[Serializable]
public class TimeLimitedEventTeamConfig
{
    public long TimeLimitedEventTeamConfigId { get; set; }
    public Guid ReplicationId { get; set; }
    public int MaxPlayersPerTeam { get; set; }
    public DateTime TeamRegistrationStartTime { get; set; }
    public DateTime TeamRegistrationEndTime { get; set; }
    public string ActiveExperiment { get; set; }
}