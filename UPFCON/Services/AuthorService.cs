using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;

namespace UPFCON.Services;




public class AuthorService(UserManager<User> userManager,IUserService userService) : IAuthorService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IUserService UserService { get; } = userService;


    public async Task<IEnumerable> GetAuthors(HttpContext httpContext)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email == null)
            throw new NotFoundException("Email not found");
        
        var user = await UserService.FindUserByEmail(email);
        
        var roles = await UserManager.GetRolesAsync(user);
        if(!roles.Contains("Author") && !roles.Contains("Admin"))
            throw new ForbiddenException("only an Author can choose the Authors ");
        
        var usersInAuthorRole = await UserManager.GetUsersInRoleAsync("Author");
        
        if(!usersInAuthorRole.Any())
            throw new NotFoundException("No Authors were found");
        
        // Filter out any users who don’t have a corresponding Author entity filled
        var authors = usersInAuthorRole
            .Where(u => u.Author != null)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Author!.Expertise,
               // u.Author!.Contributions
            });

        return authors;
    }
}