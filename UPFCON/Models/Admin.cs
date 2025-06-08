namespace UPFCON.Models;

public class Admin : User
{
    public bool PasswordChanged { get; set; } = false;
    public IList<Diploma> VerifiedDiplomas { get; set; } = new List<Diploma>();
}