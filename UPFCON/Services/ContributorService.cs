using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Requests;

namespace UPFCON.Services;



public class ContributorService(UpfconContext context, IUserService userService,
    UserManager<User> userManager) : IContributorService
{
    private UpfconContext Context { get; } = context;
    private IUserService UserService { get; } = userService;
    private UserManager<User> UserManager { get; } = userManager;
    
    

    public async Task CreateContributorsAsync(HttpContext httpContext, Paper paper, 
        IList<ContributorDto> contributorDtos)
    {
        var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null)
            throw new NotFoundException("Email not found");
        
        var loggedUser = await UserService.FindUserByEmail(email);
        
        var isAuthor = await UserManager.IsInRoleAsync(loggedUser,"Author");
        
        if(!isAuthor)
            throw new ForbiddenException("Forbidden : only an Author can create a paper ");

        var loggedAuthor = await Context.Authors.FirstOrDefaultAsync(u => u.User.Email == email)
                           ?? throw new NotFoundException("HeadAuthor not found");
        
        var contributions = new List<Contribution>();

        var headAuthor = new Contribution()
        {
            AuthorId = loggedUser.Id,
            Author = loggedAuthor,
            PaperId = paper.Id,
            Paper = paper,
            Role = Enum.GetName(ContributorRole.HeadAuthor)!
        };
        contributions.Add(headAuthor);

        foreach (var contributorDto in contributorDtos)
        {
            var contributorAuthor = await Context.Authors.FirstOrDefaultAsync(u => u.UserId == contributorDto.AuthorId)
                                    ?? throw new NotFoundException($"Contributor with ID {contributorDto.AuthorId} not found");
            
            /* var p = Context.Papers.FirstOrDefault(u => u.Id == contributorDto.PaperId);
            if (p == null)
                throw new NotFoundException("Paper not found");*/
            
            var contributor = new Contribution()
            {
                AuthorId = contributorDto.AuthorId,
                Author = contributorAuthor,
                PaperId = paper.Id,
                Paper = paper,
                Role = Enum.GetName(ContributorRole.Contributor)!
            };
            contributions.Add(contributor);
        }
        await Context.AddRangeAsync(contributions);
        await Context.SaveChangesAsync();
    }
}