namespace asdfasdf;

[Serializable]
public class BacktraceConfig
{
    public int ReportBudget { get; set; }
    public int FilterType { get; set; }
    public int SampleRate { get; set; }
    public int LogLineCount { get; set; }
    public int CaptureNativeCrashes { get; set; }
    public int AMRThresholdMS { get; set; }
    public int MessageCount { get; set; }
    public string MessageRegex { get; set; }
    public string VersionRegex { get; set; }
}