namespace asdfasdf;

[Serializable]
public class CustomAvatarItems_CreationAllowed
{
    public Error err { get; set; }
    public string error { get; set; }
    public string error_id { get; set; }
    public bool success { get; set; }
}

[Serializable]
public class Error
{
    public string id { get; set; }
    public string message { get; set; }
}