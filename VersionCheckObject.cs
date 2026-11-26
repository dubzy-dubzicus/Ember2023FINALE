namespace asdfasdf;

[Serializable]
public class VersionCheckObject
{
    public int VersionStatus { get; set; }
    public int UpdateNotificationStage { get; set; }
    /// <summary>
    /// Means this version has it's own instances
    /// </summary>
    public bool IsVersionIslanded { get; set; }
    /// <summary>
    /// Means this version can't play with people from other platforms
    /// </summary>
    public bool IsCrossPlayDisabled { get; set; }
}