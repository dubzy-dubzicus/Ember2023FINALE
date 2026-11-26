namespace asdfasdf;

[Serializable]
public class PlayerSettings
{
    public List<PlayerSetting> settings { get; set; }
}

[Serializable]
public class PlayerSetting
{
    public string Key { get; set; }
    public string Value { get; set; }
}