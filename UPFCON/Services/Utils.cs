using System.Text;
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
        if (result.Succeeded)
            return;

        var i = 1;
    
        _logger.LogError($"{cause}");
        foreach (var error in result.Errors)
            _logger.LogError($"Error {i++}:\nErrorCode: {error.Code}\n Description: {error.Description}");
    }

    public void LogInformation(string info)
    {
        _logger.LogInformation(info);
    }

    public string GenerateRandomPassword()
    {
        Random rd = new Random();
        int len = rd.Next(8, 255);

        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        // Ensure required character types
        var password = new StringBuilder();
        password.Append(upper[rd.Next(upper.Length)]);
        password.Append(lower[rd.Next(lower.Length)]);
        password.Append(digits[rd.Next(digits.Length)]);
        password.Append(special[rd.Next(special.Length)]);

        // Fill remaining characters randomly
        string allChars = upper + lower + digits + special;
        for (int i = password.Length; i < len; i++)
        {
            password.Append(allChars[rd.Next(allChars.Length)]);
        }

        // Shuffle to prevent predictable order
        return new string(password.ToString().OrderBy(c => rd.Next()).ToArray());
    }
}