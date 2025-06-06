using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Requests;

namespace UPFCON.Controllers;

[Authorize]
[Route("/api/v1/users")]
public class UserController(IUtils utils, IUserService userService) : Controller
{
    private IUtils Utils { get; } = utils;
    private IUserService UserService { get; } = userService;

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("profile")]
    public async Task<IActionResult> GetProfileAsync()
    {
        var user = await UserService.GetFromJwtEmailClaim(HttpContext);
        
        return Ok(UserProfileDto.FromUser(user));
    }

    // [HttpPost]
    // public async Task<IActionResult> EditProfileAsync([FromBody] UserProfileDto userProfileDto)
    // {
    //     var user = await UserService.GetFromJwtEmailClaim(HttpContext);
    //     
    // }
}