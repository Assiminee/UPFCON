using System.Globalization;
using System.Security.Claims;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UPFCON.Authorization;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Models.DTOs;
using UPFCON.Requests;
using UPFCON.Settings;

namespace UPFCON.Services;

public class UserService(
    UserManager<User> userManager, IDiplomaService diplomaService,
    IUtils utils, IEmailSender emailSender, IAuth authService,
    IOptions<EmailChangeSettings> emailChangeSettings,
    IOptions<RegistrationEmailSettings> registrationEmailSettings,
    UpfconContext context)
    : IUserService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IDiplomaService DiplomaService { get; } = diplomaService;
    private IUtils Utils { get; } = utils;
    private IEmailSender EmailSender { get; } = emailSender;
    private IAuth AuthService { get; } = authService;
    private IOptions<EmailChangeSettings> EmailChangeSettings { get; } = emailChangeSettings;
    private IOptions<RegistrationEmailSettings> RegistrationEmailSettings { get; } = registrationEmailSettings;
    private UpfconContext Context { get; } = context;

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

    public async Task SendConfirmationEmailAsync(User user, string confirmationLink, bool isEmailChange)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentNullException(nameof(user.Email));

        string body = isEmailChange ? EmailChangeSettings.Value.Body : RegistrationEmailSettings.Value.Body;
        string subject = isEmailChange ? EmailChangeSettings.Value.Subject : RegistrationEmailSettings.Value.Subject;
        
        body = body.Replace("{{user}}", user.FullName)
            .Replace("{{confirmationLink}}", confirmationLink);
        
        await EmailSender.SendEmailAsync(user.Email, subject, body);
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
        
        var user = await FindUserByEmail(email);
        await Context.Entry(user).Reference(u => u.Author).LoadAsync();
        await Context.Entry(user).Reference(u => u.Chairman).LoadAsync();
        await Context.Entry(user).Reference(u => u.Attendee).LoadAsync();
        
        return user;
    }

    public async Task EditUserPasswordAsync(User user, ChangePasswordDto passwords)
    {
        var res = await UserManager.ChangePasswordAsync(
            user, passwords.OldPassword, passwords.NewPassword
        );
        
        Utils.LogInformation($"Old password {passwords.OldPassword}");
        Utils.LogInformation($"New password {passwords.NewPassword}");
        
        if (!res.Succeeded)
            throw new InvalidLoginCredentialsException("Incorrect password");
    }
    
    public async Task<IdentityResult> EditUserAsync(User user, UserProfileDto userProfileDto)
    {
        user.Description = userProfileDto.Description;
        user.FirstName = userProfileDto.FirstName;
        user.LastName = userProfileDto.LastName;
        user.Birthdate = userProfileDto.Birthdate;
        user.Address = userProfileDto.Address;
        user.PhoneNumber = userProfileDto.PhoneNumber;
        
        Utils.LogInformation($"Author expertise: {user.Author?.Expertise}");
        
        if (user.Author != null)
            user.Author.Expertise = userProfileDto.Expertise ?? "";
        
        var res = await UserManager.UpdateAsync(user);
        
        Utils.LogErrors(res, "Failed to update user profile information");
        
        if (!res.Succeeded)
            throw new Exception("Failed to update user profile information");

        return res;
    }

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