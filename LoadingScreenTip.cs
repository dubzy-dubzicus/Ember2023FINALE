namespace asdfasdf;

[Serializable]
public class LoadingScreenTip
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public object[] RoomNames { get; set; }
    public int Context { get; set; }
    public int InputType { get; set; }
    public int Visibility { get; set; }
    public bool AllowCycling { get; set; }
    public bool RestrictToNewUsers { get; set; }
    public string ImageName { get; set; }
    public int PlatformMask { get; set; }
    public DateTime CreatedAt { get; set; }
}