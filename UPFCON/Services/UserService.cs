using System.Globalization;
using System.Security.Claims;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity;
using UPFCON.Authorization;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.DTOs;
using UPFCON.Requests;

namespace UPFCON.Services;

public class UserService(
    UserManager<User> userManager, IDiplomaService diplomaService,
    IUtils utils, IEmailSender emailSender, IAuth authService)
    : IUserService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IDiplomaService DiplomaService { get; } = diplomaService;
    private IUtils Utils { get; } = utils;
    private IEmailSender EmailSender { get; } = emailSender;
    private IAuth AuthService { get; } = authService;

    public async Task<(User user, IEnumerable<string> roles)> CreateUserAsync(
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

        // Utils.LogInformation($"AccountStatus {user.AccountStatus}");

        CreateAuthorAttendeeChairman(registrationDto, user);
        var res = await UserManager.CreateAsync(user, registrationDto.Password);
        
        Utils.LogErrors(res, "Encountered errors when creating a user");

        if (!res.Succeeded)
        {
            var exception = res.Errors.Any(e => 
                e.Code.Equals("DuplicateUserName") || e.Code.Equals("DuplicateEmail")
            );

            if (exception)
                throw new DuplicateEmailException($"The email {registrationDto.Email} already exists.");
        }
        
        return (user, roles);
    }
    
    
    public async Task AddRolesAsync(User user, IEnumerable<string> roles)
    {
        var res = await UserManager.AddToRolesAsync(user, roles);

        Utils.LogErrors(res, "Encountered errors when adding user roles");
    }

    public async Task SendConfirmationEmailAsync(User user, string confirmationLink)
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
                      "you've provided (diplomas.)</p>" +
                      "<p>If you've only registered as an attendee, the confirmation process does not" +
                      " concern you.</p>" +
                      "<p>Thank you again for choosing UPFCON!</p>" +
                      "<p>Enjoy this journey</p>" +
                      "<p>UPFCON team</p>";
        
        await EmailSender.SendEmailAsync(user.Email, subject, body);
    }

    public async Task<User> FindUserById(string id)
    {
        var user = await UserManager.FindByIdAsync(id);

        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }

    public async Task<IEnumerable<Claim>> GenerateUserClaimsAsync(User user)
    {
        var roles = await UserManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        };
        
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return claims;
    }
    
    public async Task<JwtToken> AuthenticateUser(LoginDto loginDto)
    {
        var user = await UserManager.FindByEmailAsync(loginDto.Email);
        
        if (user == null)
            throw new InvalidLoginCredentialsException("Invalid login credentials");
        
        var validPassword = await UserManager.CheckPasswordAsync(user, loginDto.Password);
        
        if (!validPassword)
            throw new InvalidLoginCredentialsException("Invalid login credentials");

        var emailConfirmed = await UserManager.IsEmailConfirmedAsync(user);
        if (!emailConfirmed)
            throw new EmailNotConfirmedException("Email not confirmed");

        var claims = await GenerateUserClaimsAsync(user);
        var expiresAt = DateTime.UtcNow.AddHours(8);
        
        var token = AuthService.GenerateJwtToken(claims, expiresAt);
        
        return new JwtToken()
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
        };
    }

    public async Task ConfirmUserAsync(User user, string token)
    {
        var res = await UserManager.ConfirmEmailAsync(user, token);

        Utils.LogErrors(res, "Failed to confirm email");

        if (!res.Succeeded)
            throw new Exception("Failed to confirm email");
    }

    public async Task<string> GenerateEmailConfirmationLinkAsync(User user)
    {
        return await EmailSender.GenerateEmailConfirmationLinkAsync(user, UserManager);
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