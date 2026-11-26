namespace asdfasdf;

[Serializable]
public class ApiResponse
{
    public string error { get; set; }
    public bool success { get; set; }
    public Object value { get; set; }
}

[Serializable]
public class ApiResponseNoValue
{
    public string error { get; set; }
    public string error_id { get; set; }
    public bool success { get; set; }
}