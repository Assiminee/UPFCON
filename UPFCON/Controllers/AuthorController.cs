using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;

namespace UPFCON.Controllers;



[ApiController]
[Route("api/v1/authors")]
public class AuthorController(IAuthorService authorService) : Controller
{
    public IAuthorService AuthorService { get; } = authorService;


    [HttpGet]
    [Route("available")]
    public async Task<IActionResult> GetAvailableAuthors()
    {
        var authors = await AuthorService.GetAuthors(HttpContext);
        
        return Ok(authors);
    }
}