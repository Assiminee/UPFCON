using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Settings;

namespace UPFCON.Services;

public class EmailSenderService(IOptions<SmtpSettings> settings) : IEmailSender
{
    private IOptions<SmtpSettings> Settings { get; } = settings;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MailMessage(Settings.Value.User, to, subject, body);
        message.IsBodyHtml = true;

        using (var client = new SmtpClient(Settings.Value.Host, Settings.Value.Port))
        {
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(
                Settings.Value.User,
                Settings.Value.Password
            );
            
            await client.SendMailAsync(message);
        }
    }
}
