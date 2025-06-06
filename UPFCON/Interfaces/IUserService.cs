using Microsoft.AspNetCore.Identity;
using UPFCON.Authorization;
using UPFCON.Models;
using UPFCON.Models.DTOs;
using UPFCON.Requests;

namespace UPFCON.Interfaces;

public interface IUserService
{
    Task<(User user, IEnumerable<string> roles)> CreateUserAsync(RegistrationDto registrationDto);
    Task AddRolesAsync(User user, IEnumerable<string> roles);
    Task SendConfirmationEmailAsync(User user, string confirmationLink, bool isEmailChange);
    Task<User> FindUserById(string id);
    Task ConfirmEmailAsync(User user, string token);
    Task<string> GenerateEmailConfirmationLinkAsync(User user);
    Task<JwtToken> AuthenticateUser(LoginDto loginDto);
    Task<User> FindUserByEmail(string email);
    Task<User> GetFromJwtEmailClaim(HttpContext httpContext);
    
    Task<IdentityResult> EditUserAsync(User user, UserProfileDto userProfileDto);
    Task EditUserPasswordAsync(User user, ChangePasswordDto passwords);
}