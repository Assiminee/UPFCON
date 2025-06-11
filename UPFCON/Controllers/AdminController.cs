using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Controllers;

[Route("/api/v1/admins")]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminService adminService, IUserService userService, IUtils utils) : Controller
{
    private IAdminService AdminService { get; } = adminService;
    private IUserService UserService { get; } = userService;
    public IUtils Utils { get; } = utils;

    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> CreatAdmin([FromBody] AdminDto adminDto)
    {
        var admin = await AdminService.CreateAdmin(adminDto);
        var confirmationLink = await UserService.GenerateEmailConfirmationLinkAsync(
            admin, "http://localhost", 4200, "activate-account"
        );

        await UserService.SendConfirmationEmailAsync(admin, confirmationLink, true);
        return Ok(new
        {
            status = 201,
            id = admin.Id
        });
    }

    [HttpGet]
    public IActionResult Get([FromQuery] int page, [FromQuery] int pageSize)
    {
        var user = HttpContext.User.IsInRole("Admin");
        Utils.LogInformation($"Is user logged in {HttpContext.User.Identity?.IsAuthenticated}");
        Utils.LogInformation($"Is user an admin? {user}");
        Utils.LogInformation($"User email {HttpContext.User.Identity?.AuthenticationType}");
        return Ok(new {
            admins = AdminService.GetAdmins(page, pageSize),
            count = AdminService.GetCount()
        });
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetAdmin([FromRoute] Guid id)
    {
        var admin = await AdminService.GetAdminById(id);
        var adminDto = AdminDto.FromAdmin(admin);
        
        return Ok(adminDto);
    }

    [HttpPut]
    [Route("{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdateAdmin([FromRoute] Guid id, [FromBody] AdminDto adminDto)
    {
        await AdminService.UpdateAdmin(id, adminDto);
        
        return Ok();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteAdmin([FromRoute] Guid id)
    {
        await AdminService.DeleteAdmin(id);
        
        return Ok();
    }
}