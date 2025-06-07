using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.DTOs;
using UPFCON.Requests;

namespace UPFCON.Controllers;

[ApiController]
[Route("/api/v1/auth")]
[AllowAnonymous]
public class AuthController(IUserService userService) : Controller
{
    private IUserService UserService { get; } = userService;

    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterAsync([FromForm] RegistrationDto registrationDto) {
        var (user, roles) = await UserService.CreateUserAsync(registrationDto);

        await UserService.AddRolesAsync(user, roles);

        var confirmationLink = await UserService.GenerateEmailConfirmationLinkAsync(user);
        await UserService.SendConfirmationEmailAsync(user, confirmationLink, false);

        return Created();
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string token)
    {
        var user = await UserService.FindUserById(userId);
        
        await UserService.ConfirmEmailAsync(user, token);
        
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await UserService.AuthenticateUser(loginDto);
        
        return Ok(token);
    }
}