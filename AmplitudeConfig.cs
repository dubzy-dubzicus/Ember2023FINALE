namespace asdfasdf;

[Serializable]
public class AmplitudeConfig
{
    public string AmplitudeKey { get; set; }
    public bool UseRudderStack { get; set; }
    public string RudderStackKey { get; set; }
    public bool UseStatSig { get; set; }
    public string StatSigKey { get; set; }
    public int StatSigEnvironment { get; set; }
}