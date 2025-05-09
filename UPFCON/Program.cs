using UPFCON.Models;

namespace UPFCON;

public class Program
{
    public static void Main()
    {
        Diploma diploma = new Diploma(
            "Genie Informatique",
            new DateTime(2026, 09, 01),
            "home",
            DiplomaVerificationStatus.PendingVerification
        );
        
        diploma.Id = Guid.CreateVersion7();
        
        User user = new User(
            "Yasmine",
            "Znatni",
            "znatni.yasmine@gmail.com",
            "+212648288553",
            new DateTime(1997, 10, 18),
            "A user",
            "60 Route Ain Chkef Hay Nouzha, Fes 30050",
            "Password@1",
            AccountStatus.PendingVerification
            );

        user.Id = Guid.CreateVersion7();
        user.Diplomas = new List<Diploma>();
        user.Diplomas.Add(diploma);
        
        Console.WriteLine(user.ToString());
        Console.WriteLine(diploma.ToString());
    }
    
}