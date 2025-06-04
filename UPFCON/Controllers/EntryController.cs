using Microsoft.AspNetCore.Mvc;

namespace UPFCON.Controllers;

[Route("/Entry")]
public class EntryController : Controller
{
    // GET
    public IActionResult Index()
    {
        return Ok(new
        {
            name = "Assimine",
            msg = "hello"
        });
    }
}