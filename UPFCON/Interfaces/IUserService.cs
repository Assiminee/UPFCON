using Microsoft.AspNetCore.Identity;
using UPFCON.Authorization;
using UPFCON.Models;
using UPFCON.Models.DTOs;
using UPFCON.Requests;

namespace UPFCON.Interfaces;

public interface IUserService
{
    Task<(IdentityResult res, User user, IEnumerable<string> roles)> CreateUserAsync(RegistrationDto registrationDto);
    Task<IdentityResult> AddRolesAsync(User user, IEnumerable<string> roles);
    Task<IdentityResult> SendConfirmationEmailAsync(User user, string confirmationLink);
    Task<User?> FindUserById(string id);
    Task<IdentityResult> ConfirmUserAsync(User user, string token);
    Task<string> GenerateEmailConfirmationLinkAsync(User user);
    Task<JwtToken> AuthenticateUser(LoginDto loginDto);
}