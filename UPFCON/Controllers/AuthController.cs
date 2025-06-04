using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Models.DTOs;

namespace UPFCON.Controllers;

[ApiController]
[Route("/api/v1/auth")]
[AllowAnonymous]
public class AuthController(UserManager<User> userManager, IUserService userService) : Controller
{
    private IUserService UserService { get; } = userService;

    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterAsync([FromForm] RegistrationDto registrationDto) {
        try
        {
            var (createUserRes, user, roles) = await UserService.CreateUserAsync(registrationDto);

            if (!createUserRes.Succeeded)
                return BadRequest(createUserRes.Errors);

            var addRolesRes = await UserService.AddRolesAsync(user, roles);

            if (!addRolesRes.Succeeded)
                return BadRequest(addRolesRes.Errors);

            var confirmationLink = await UserService.GenerateEmailConfirmationLinkAsync(user);
            var confirmationEmailRes = await UserService.SendConfirmationEmailAsync(user, confirmationLink);

            if (!confirmationEmailRes.Succeeded)
                return BadRequest(confirmationEmailRes.Errors);

            return Created();
        }
        catch (InvalidFileException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidUserRoleException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentNullException)
        {
            return BadRequest("Missing user email");
        }
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string token)
    {
        var user = await UserService.FindUserById(userId);
        
        if (user == null)
            return NotFound();
        
        var res = await UserService.ConfirmUserAsync(user, token);
        if (!res.Succeeded)
            return BadRequest(res.Errors);
        
        return Ok();
    }
}