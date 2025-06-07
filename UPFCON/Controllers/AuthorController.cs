using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;

namespace UPFCON.Controllers;



[ApiController]
[Route("api/v1/authors")]
[Authorize]
public class AuthorController(IAuthorService authorService) : Controller
{
    private IAuthorService AuthorService { get; } = authorService;


    [HttpGet]
    public async Task<IActionResult> GetAvailableAuthors()
    {
        var authors = await AuthorService.GetAuthors(HttpContext);
        
        return Ok(authors);
    }
}