using Microsoft.AspNetCore.Identity;
using UPFCON.Models;

namespace UPFCON.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
    Task<string> GenerateEmailConfirmationLinkAsync(User user, UserManager<User> userManager);
}