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
    Task SendConfirmationEmailAsync(User user, string confirmationLink, bool isAccountActivation);
    Task<User> FindUserById(string id);
    Task ConfirmEmailAsync(User user, string token);
    Task<string> GenerateEmailConfirmationLinkAsync(User user, string host, int port, string uri);
    Task<JwtToken> AuthenticateUser(LoginDto loginDto);
    Task<User> FindUserByEmail(string email);
    Task<User> GetFromJwtEmailClaim(HttpContext httpContext);
    
    Task<IdentityResult> EditUserAsync(User user, UserProfileDto userProfileDto);
    Task EditUserPasswordAsync(User user, ChangePasswordDto passwords);
    Task<bool> HasRole(User user, string role);
    Task SetPasswordAsync(User user, string password);
    Task<(IList<UserDto>, int)> GetUsersAsync(int page, int pageSize);
    Task DeleteUserAsync(string id);
}