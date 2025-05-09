namespace UPFCON.Models;

public enum AccountStatus
{
    Verified,
    PendingVerification,
    Rejected,
    Deleted
}

public class Admin : User
{
    public IList<Diploma> VerifiedDiplomas { get; set; } = new List<Diploma>();
}