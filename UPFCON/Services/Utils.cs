using Microsoft.AspNetCore.Identity;
using UPFCON.Interfaces;

namespace UPFCON.Services;

public class Utils(ILogger<Utils> logger) : IUtils
{
    private readonly ILogger<Utils> _logger = logger;
    
    public IList<string> CapitalizeStrings(IList<string> stringList)
    {
        return stringList
            .Select(str => string.IsNullOrWhiteSpace(str) ? str : char.ToUpperInvariant(str[0]) + str.Substring(1))
            .ToList();
    }

    public void LogErrors(IdentityResult result, string cause)
    {
        if (!result.Succeeded)
        {
            var i = 1;
        
            _logger.LogError($"{cause}");
            foreach (var error in result.Errors)
                _logger.LogError($"Error {i++}:\nErrorCode: {error.Code}\n Description: {error.Description}");
        }
    }

    public void LogInformation(string info)
    {
        _logger.LogInformation(info);
    }
}