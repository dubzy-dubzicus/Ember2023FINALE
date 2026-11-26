namespace asdfasdf;

[Serializable]
public class LoginResponse
{
    public string Error { get; set; }
    public string ErrorDescription { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Key { get; set; }
    public SecurityCodeHint CodeHint { get; set; }
    public string LoginContext { get; set; }
    public string Challenge { get; set; }
}

[Serializable]
public class SecurityCodeHint
{
    public string Text { get; set; }
    public int Length { get; set; }
    public bool AllowAlpha { get; set; }
}