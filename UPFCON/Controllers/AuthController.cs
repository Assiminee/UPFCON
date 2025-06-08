using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.DTOs;
using UPFCON.Requests;

namespace UPFCON.Controllers;

[ApiController]
[Route("/api/v1/auth")]
[AllowAnonymous]
public class AuthController(IUserService userService, IAdminService adminService, IUtils utils) : Controller
{
    private IUserService UserService { get; } = userService;
    public IUtils Utils { get; } = utils;

    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterAsync([FromForm] RegistrationDto registrationDto) {
        var (user, roles) = await UserService.CreateUserAsync(registrationDto);

        await UserService.AddRolesAsync(user, roles);

        var confirmationLink = await UserService.GenerateEmailConfirmationLinkAsync(
            user, "http://localhost",
            5280, "api/v1/auth/confirm-email"
            );
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

    [HttpPost("activate-account/{userId}")]
    [Consumes("application/json")]
    public async Task<IActionResult> ActivateAccount([FromRoute] string userId, AccountActivationPayload payload)
    {
        if (payload.Password != payload.ConfirmPassword)
        {
            return BadRequest(new
            {
                status = 400,
                message = "Passwords do not match"
            });
        }

        var user = await UserService.FindUserById(userId);

        var isAdmin = await UserService.HasRole(user, Enum.GetName(Roles.Admin) ?? "Admin");
        var isBoardDirector = await UserService.HasRole(user, Enum.GetName(Roles.BoardDirector) ?? "BoardDirector");

        if (!isAdmin && !isBoardDirector)
        {
            return Unauthorized(new
            {
                status = 401,
                message = "Access denied"
            });
        }

        if (user.EmailConfirmed)
        {
            return Ok(new
            {
                status = 200,
                message = "Email already confirmed"
            });
        }

        Utils.LogInformation($"Token: {payload.Token}");
        Utils.LogInformation($"UserId: {userId}");
        

        await UserService.ConfirmEmailAsync(user,  WebUtility.UrlDecode(payload.Token));
        await UserService.SetPasswordAsync(user, payload.Password);
        return Ok(new
            {
                status = 200,
                message = isAdmin ? "Admin" : "BoardDirector",
            }
        );
    }
    // CfDJ8E6VHeVNoLtAgf63gzHeczyXFL0pIiRtEy0Clc3x5uQRvBVYeUmUPCICEhMucX%2Fp5asHxldfpMpTcnt0fNQeK%2BuGWfgkmRNB4fZ6oEGQBWcBk3ciLTVGrDRkc24Y1eSV5l%2B5WF1IxaK%2BEayZYTUIWMT8cKtnD4NjXULTGEAHTNzZnxn77a1upTB5bTiy%2FIUQTzzICroQTWYcLjrGnsvKZHSbZoapPCoA%2F5SbF%2BhuLH%2FVmkFLuWCDw1IDbueZEp23Gg%3D%3D
    // CfDJ8E6VHeVNoLtAgf63gzHeczyXFL0pIiRtEy0Clc3x5uQRvBVYeUmUPCICEhMucX%2Fp5asHxldfpMpTcnt0fNQeK%2BuGWfgkmRNB4fZ6oEGQBWcBk3ciLTVGrDRkc24Y1eSV5l%2B5WF1IxaK%2BEayZYTUIWMT8cKtnD4NjXULTGEAHTNzZnxn77a1upTB5bTiy%2FIUQTzzICroQTWYcLjrGnsvKZHSbZoapPCoA%2F5SbF%2BhuLH%2FVmkFLuWCDw1IDbueZEp23Gg%3D%3D
    // CfDJ8E6VHeVNoLtAgf63gzHeczyzvw74ljviZHf%2BvPuVfNGTC%2BTg4rm5mma5bEvwjHqCXIPzCBkYTp%2B6npLJCmSfVONLzeMGUeKYg%2FxHItgrv2pGS%2FuXFrlHZhcH15xL38bT5%2BnF7bdta8b6uN4VTDaIQpWUKae8aPTDUHEYLLamvlTlpEuumJ1cycMiYRGGcFHOSSpolAKBBuYGrHvCJK5oniSmrfx9RcOhGCmln8ul0nn64dxQvr%2F4cmnqErTLHQWIcw%3D%3D
    // CfDJ8E6VHeVNoLtAgf63gzHeczyzvw74ljviZHf%2BvPuVfNGTC%2BTg4rm5mma5bEvwjHqCXIPzCBkYTp%2B6npLJCmSfVONLzeMGUeKYg%2FxHItgrv2pGS%2FuXFrlHZhcH15xL38bT5%2BnF7bdta8b6uN4VTDaIQpWUKae8aPTDUHEYLLamvlTlpEuumJ1cycMiYRGGcFHOSSpolAKBBuYGrHvCJK5oniSmrfx9RcOhGCmln8ul0nn64dxQvr%2F4cmnqErTLHQWIcw%3D%3D
}