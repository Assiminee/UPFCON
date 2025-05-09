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
    public Admin(
        string fname, string lname, string email, string phone,
        DateTime birthdate, string description, string address,
        string pwd, AccountStatus accountStatus
        ) : base(fname, lname, email, phone, birthdate, description, address, pwd, accountStatus)
    { }
    
    public IList<Diploma> VerifiedDiplomas { get; set; }
}