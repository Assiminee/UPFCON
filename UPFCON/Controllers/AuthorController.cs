using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
using UPFCON.Requests;

namespace UPFCON.Controllers;



[ApiController]
[Route("api/v1/authors")]
[Authorize]
public class AuthorController(IAuthorService authorService, IUserService userService) : Controller
{
    private IAuthorService AuthorService { get; } = authorService;
    
    private IUserService UserService { get; } = userService;
    
    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] List<AuthorRegisterDto> authorRegisterDtos) {
        
        var (users, roles) = await AuthorService.CreateAuthors(HttpContext,
            authorRegisterDtos);

        foreach (var user in users)
        {
            await UserService.AddRolesAsync(user, roles);
            
            var confirmationLink = await UserService.GenerateEmailConfirmationLinkAsync(user);
            
            await UserService.SendConfirmationEmailAsync(user, confirmationLink, false);
        }
        
        return Created();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAvailableAuthors()
    {
        var authors = await AuthorService.GetAuthors(HttpContext);
        
        return Ok(authors);
    }
}