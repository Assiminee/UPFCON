using Microsoft.EntityFrameworkCore;
using UPFCON.Models.Context;

namespace UPFCON.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/v1/chairmen")]
[Authorize]                 
public class ChairmanController : ControllerBase
{
    private readonly UpfconContext _ctx;
    public ChairmanController(UpfconContext ctx) => _ctx = ctx;
    
    [HttpGet]
    public IActionResult Search([FromQuery] string q = "")
    {
        var data = _ctx.Chairmans        
            .Include(c => c.User) 
            .Where(c => (c.User.FirstName + " " + c.User.LastName)
                .Contains(q))
            .Select(c => new {
                id   = c.UserId,
                name = c.User.FirstName + " " + c.User.LastName,
                email = c.User.Email                       

            })
            .Take(10)
            .ToList();

        return Ok(data);
    }
}
