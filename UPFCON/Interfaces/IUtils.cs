using Microsoft.AspNetCore.Identity;

namespace UPFCON.Interfaces;

public interface IUtils
{
    IList<string> CapitalizeStrings(IList<string> stringList);
    void LogErrors(IdentityResult result, string cause);
    void LogInformation(string info);
}