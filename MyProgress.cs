namespace asdfasdf;

[Serializable]
public class MyProgress
{
    public List<ObjectiveGroup> ObjectiveGroups { get; set; }
    public List<Objective> Objectives { get; set; }
}

[Serializable]
public class ObjectiveGroup
{
    public DateTime ClearedAt { get; set; }
    public int Group { get; set; }
    public bool IsCompleted { get; set; }
    public bool RequiresCompleteOnServer { get; set; }
}

[Serializable]
public class Objective
{
    public int Group { get; set; }
    public bool HasClaimedReward { get; set; }
    public int Index { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsRewarded { get; set; }
    public int Progress { get; set; }
    public int VisualProgress { get; set; }
}