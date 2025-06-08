using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Controllers;

[Route("/api/v1/admins")]
public class AdminController(IAdminService adminService, IUserService userService) : Controller
{
    public IAdminService AdminService { get; } = adminService;
    public IUserService UserService { get; } = userService;

    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> CreatAdmin([FromBody] AdminDto adminDto)
    {
        var admin = await AdminService.CreateAdmin(adminDto);
        var confirmationLink = await UserService.GenerateEmailConfirmationLinkAsync(
            admin, "http://localhost", 4200, "activate-account"
        );

        await UserService.SendConfirmationEmailAsync(admin, confirmationLink, true);
        return Created();
    }
}