using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Models.DTOs;

namespace UPFCON.Controllers;

[ApiController]
[Route("/api/v1/auth")]
[AllowAnonymous]
public class AuthController : Controller
{
    private readonly UpfconContext _context;
    private readonly UserManager<User> _userManager;

    public AuthController(UpfconContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // [HttpPost("login")]
    // public async Task<IActionResult> login([FromBody] LoginInfo creds)
    // {
    //     if (string.IsNullOrWhiteSpace(creds.Email))
    //         return BadRequest(new { message = "Email is required" });
    //     
    //     if (string.IsNullOrWhiteSpace(creds.Password))
    //         return BadRequest(new { message = "Password is required" });
    //
    //     var user = await _userManager.FindByEmailAsync(creds.Email);
    //     if (user == null)
    //         return Unauthorized(new { message = "Invalid email" });
    //
    //     bool passwordValid = await _userManager.CheckPasswordAsync(user, creds.Password);
    //     if (!passwordValid)
    //         return Unauthorized(new { message = "Invalid password" });
    //     
    //     if (!signInResult.Succeeded)
    //         return Unauthorized(new { message = "Incorrect login credentials" });
    //
    //
    //     var token = GenerateJwtForUser(User);
    //     return NotFound();
    // }
}