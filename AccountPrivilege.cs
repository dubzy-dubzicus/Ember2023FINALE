namespace asdfasdf;

[Serializable]
public class AccountPrivilege
{
    public int AccountId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime EndDate { get; set; }
    public Int32 AssociatedAccountid { get; set; }
    public string AssociatedAccountUsername { get; set; }
    public ReportCategory ReportCategory { get; set; }
    public string DisplayReason { get; set; }
}