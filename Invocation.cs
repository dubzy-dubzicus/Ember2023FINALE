namespace asdfasdf;

public class Invocation
{
    public int type { get; set; }
    public string invocationId { get; set; }
    public bool nonblocking { get; set; }
    public string target { get; set; }
    public Arguments[] arguments { get; set; }
}

public class Arguments
{
    public int[] PlayerIds { get; set; }
}