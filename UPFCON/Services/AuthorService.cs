using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.Context;

namespace UPFCON.Services;




public class AuthorService(UserManager<User> userManager,IUserService userService,UpfconContext context) : IAuthorService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IUserService UserService { get; } = userService;
    
    private UpfconContext Context { get; } = context;


    public async Task<IEnumerable> GetAuthors(HttpContext httpContext)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        
        if (email == null)
            throw new NotFoundException("Email not found");
        
        var loggedUser = await UserService.FindUserByEmail(email);
        
        var role = await UserManager.IsInRoleAsync(loggedUser,"Author");
        
        if(!role)
            throw new ForbiddenException("Forbidden : only a HeadAuthor can choose the Contributors ");

        var usersInAuthorRole = await UserManager.GetUsersInRoleAsync("Author");

        var authors = new List<object>();

        foreach (var user in usersInAuthorRole)
        {
            // Explicitly load the Author navigation property
            await Context.Entry(user).Reference(u => u.Author).LoadAsync();

            // Only add users who have an Author profile
            if (user.Author != null)
            {
                authors.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Author.Expertise
                });
            }
        }
        
        authors.Remove(loggedUser); // doesnt work for some fking reason
        
        return authors;
    }
}