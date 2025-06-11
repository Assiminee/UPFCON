using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Requests;

namespace UPFCON.Services;




public class AuthorService(UserManager<User> userManager,IUserService userService,
    UpfconContext context, IUtils utils) : IAuthorService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IUserService UserService { get; } = userService;
    
    private UpfconContext Context { get; } = context;
    
    private IUtils Utils { get; } = utils;


    public async Task<(IList<User> users, IEnumerable<string> roles)> CreateAuthors(HttpContext httpContext, List<AuthorRegisterDto> authorRegisterDtos)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null)
            throw new NotFoundException("Email not found");
        
        var loggedUser = await UserService.FindUserByEmail(email);
        
        var isAuthor = await UserManager.IsInRoleAsync(loggedUser,"Author");
        
        if(!isAuthor)
            throw new ForbiddenException("Forbidden : only an Author can choose the Contributors ");

        // role will always be an author, other roles can be added later when modifying the profile
        List<string> rolesHolder =
        [
            "Author"
        ];

        IEnumerable<string> roles = Utils.CapitalizeStrings(rolesHolder);
        
        foreach (var role in roles)
        {
            if (!Enum.GetNames<Roles>().Contains(role))
                throw new InvalidUserRoleException($"Invalid role {role}");
        }
        
        List<User> users = new List<User>();

        foreach (var authorDto in authorRegisterDtos)
        {
            var user = new User
            {
                FirstName = authorDto.FirstName,
                LastName = authorDto.LastName,
                Email = authorDto.Email,
                UserName = authorDto.Email,
                Birthdate = DateTime.Now,
                Address = string.Empty,
                Attendee = new Attendee(),
                Author = new Author(authorDto.Expertise)
            };

            var res = await UserManager.CreateAsync(user,"Password@1");
        
            Utils.LogErrors(res, "Encountered errors when creating a user");

            if (!res.Succeeded)
            {
                var exception = res.Errors.Any(e => 
                    e.Code.Equals("DuplicateUserName") || e.Code.Equals("DuplicateEmail")
                );

                if (exception)
                    throw new DuplicateEmailException($"The email {authorDto.Email} already exists.");
            }
            users.Add(user);
        } 
        return (users, roles);
    }

    public async Task<IEnumerable> GetAuthors(HttpContext httpContext)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        
        if (email == null)
            throw new NotFoundException("Email not found");
        
        var loggedUser = await UserService.FindUserByEmail(email);
        
        var role = await UserManager.IsInRoleAsync(loggedUser,"Author");
        
        if(!role)
            throw new ForbiddenException("Forbidden : only an Author can choose the Contributors ");

        var usersInAuthorRole = await UserManager.GetUsersInRoleAsync("Author");

        var authors = new List<object>();

        foreach (var user in usersInAuthorRole)
        {
            
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