using System.Security.Policy;
using Microsoft.AspNetCore.Identity;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.DTOs;

namespace UPFCON.Services;

public class UserService(UserManager<User> userManager, IDiplomaService diplomaService, IUtils utils, IEmailSender emailSender)
    : IUserService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IDiplomaService DiplomaService { get; } = diplomaService;
    private IUtils Utils { get; } = utils;
    public IEmailSender EmailSender { get; } = emailSender;

    public async Task<(IdentityResult res, User user, IEnumerable<string> roles)> CreateUserAsync(
        RegistrationDto registrationDto)
    {
        IList<Diploma> diplomas = await DiplomaService.CreateDiplomaListAsync(registrationDto.Diplomas);
        IEnumerable<string> roles = Utils.CapitalizeStrings(registrationDto.Roles);

        foreach (var role in roles)
        {
            if (!Enum.GetNames<Roles>().Contains(role))
                throw new InvalidUserRoleException($"Invalid role {role}");
        }

        var user = new User
        {
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName,
            Birthdate = registrationDto.Birthdate,
            Address = registrationDto.Address,
            Email = registrationDto.Email,
            UserName = registrationDto.Email,
            PhoneNumber = registrationDto.PhoneNumber,
            Diplomas = diplomas
        };

        Utils.LogInformation($"AccountStatus {user.AccountStatus}");

        CreateAuthorAttendeeChairman(registrationDto, user);
        var res = await UserManager.CreateAsync(user, registrationDto.Password);
        
        Utils.LogErrors(res, "Encountered errors when creating a user");

        return (res, user, roles);
    }
    
    

    public async Task<IdentityResult> AddRolesAsync(User user, IEnumerable<string> roles)
    {
        var res = await UserManager.AddToRolesAsync(user, roles);

        Utils.LogErrors(res, "Encountered errors when adding user roles");

        return res;
    }

    public async Task<IdentityResult> SendConfirmationEmailAsync(User user, string confirmationLink)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentNullException(nameof(user.Email));
        
        string subject = "UPFCON Confirmation Email";
        string body = "<h1>Thank you for choosing UPFCON</h1><br/>" +
                      "<p>We are delight to welcome you to UPFCON!</p>" +
                      "<p>In order to access your profile, you must confirm your email<p>" +
                      $"<p>Please follow this <a href={confirmationLink}>link</a></p>" +
                      "<p>Once your account is confirmed, a validation process will commence " +
                      "where one of our admins will confirm your identity as well as the documents " +
                      "you've provided (diplomas)</p>" +
                      "<p>If you've only registered as an attendee, the confirmation process does not" +
                      " concern you.</p>" +
                      "<p>Thank you again for choosing UPFCON!</p>" +
                      "<p>Enjoy this journey</p>" +
                      "<p>UPFCON team</p>";
        
        await EmailSender.SendEmailAsync(user.Email, subject, body);
        return IdentityResult.Success;
    }

    public async Task<User?> FindUserById(string id)
    {
        return await UserManager.FindByIdAsync(id);
    }

    public async Task<IdentityResult> ConfirmUserAsync(User user, string token)
    {
        var res = await UserManager.ConfirmEmailAsync(user, token);

        Utils.LogErrors(res, "Failed to confirm email");

        return res;
    }

    public async Task<string> GenerateEmailConfirmationLinkAsync(User user)
    {
        var link = await EmailSender.GenerateEmailConfirmationLinkAsync(user, UserManager);

        Utils.LogInformation(link);
        
        return link;
    }

    private static void CreateAuthorAttendeeChairman(RegistrationDto registrationDto, User user)
    {
        var authorRole = Enum.GetName(Roles.Author) ?? "";
        var chairmanRole = Enum.GetName(Roles.Chairman) ?? "";
        user.Attendee = new Attendee();

        if (registrationDto.Roles.Contains(authorRole))
            user.Author = new Author(registrationDto.Expertise ?? string.Empty);

        if (registrationDto.Roles.Contains(chairmanRole))
            user.Chairman = new Chairman(registrationDto.IsInternal ?? true);
    }
}