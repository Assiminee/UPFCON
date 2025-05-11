namespace UPFCON.Models;

public class Admin : User
{
    public IList<Diploma> VerifiedDiplomas { get; set; } = new List<Diploma>();
}