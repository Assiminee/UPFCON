using System.Globalization;
using System.Security.Claims;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UPFCON.Authorization;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.DTOs;
using UPFCON.Requests;
using UPFCON.Settings;

namespace UPFCON.Services;

public class UserService(
    UserManager<User> userManager, IDiplomaService diplomaService,
    IUtils utils, IEmailSender emailSender, IAuth authService,
    IOptions<EmailChangeSettings> emailChangeSettings,
    IOptions<RegistrationEmailSettings> registrationEmailSettings)
    : IUserService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IDiplomaService DiplomaService { get; } = diplomaService;
    private IUtils Utils { get; } = utils;
    private IEmailSender EmailSender { get; } = emailSender;
    private IAuth AuthService { get; } = authService;
    public IOptions<EmailChangeSettings> EmailChangeSettings { get; } = emailChangeSettings;
    public IOptions<RegistrationEmailSettings> RegistrationEmailSettings { get; } = registrationEmailSettings;

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
        
        string body = RegistrationEmailSettings.Value.Body
            .Replace("{{User}}", user.FullName)
            .Replace("{{ConfirmationLink}}", confirmationLink);
        
        string subject = RegistrationEmailSettings.Value.Subject;
        
        await EmailSender.SendEmailAsync(user.Email, subject, body);
    }
    
    public async Task SendEmailChangeConfirmationAsync(User user, string confirmationLink)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentNullException(nameof(user.Email));
        
        string body = RegistrationEmailSettings.Value.Subject
            .Replace("{{User}}", user.FullName)
            .Replace("{{ConfirmationLink}}", confirmationLink);
        
        await EmailSender.SendEmailAsync(
            user.Email, RegistrationEmailSettings.Value.Subject, body
        );
    }

    public async Task<User> FindUserById(string id)
    {
        var user = await UserManager.FindByIdAsync(id);

        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }

    public async Task<User> GetFromJwtEmailClaim(HttpContext httpContext)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        
        if (email == null)
            throw new NotFoundException("Email not found");
        
        return await FindUserByEmail(email);
    }

    // public async Task<IdentityResult> EditUserAsync(User user, UserProfileDto userProfileDto)
    // {
    //     bool emailChanged = user.Email != null && !user.Email.ToLower().Equals(userProfileDto.Email.ToLower());
    //     
    //     user.Description = userProfileDto.Description;
    //     user.FirstName = userProfileDto.FirstName;
    //     user.LastName = userProfileDto.LastName;
    //     user.Birthdate = userProfileDto.Birthdate;
    //     user.Address = userProfileDto.Address;
    //     user.Email = userProfileDto.Email;
    //     user.PhoneNumber = userProfileDto.PhoneNumber;
    //     
    //     if (user.Author != null)
    //         user.Author.Expertise = userProfileDto.Expertise ?? "";
    //     
    //     var updated = await UserManager.UpdateAsync(user);
    //
    //     if (emailChanged)
    //     {
    //         var emailChangeToken = await UserManager.GenerateChangeEmailTokenAsync(user, userProfileDto.Email);
    //         var res = await UserManager.ChangeEmailAsync(user, userProfileDto.Email, emailChangeToken);
    //         if (res.Succeeded)
    //         {
    //             
    //         }
    //     }
    //
    //     return updated;
    // }

    public async Task<User> FindUserByEmail(string email)
    {
        var user = await UserManager.FindByEmailAsync(email);

        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }

    private async Task<IEnumerable<Claim>> GenerateUserClaimsAsync(User user)
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

    public async Task ConfirmEmailAsync(User user, string token)
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