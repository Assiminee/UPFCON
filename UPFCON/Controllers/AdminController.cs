using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
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
    public async Task<IActionResult> CreatAdmin([FromBody] UserDto userDto)
    {
        var admin = await AdminService.CreateAdmin(userDto);
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
    public async Task<IActionResult> Get([FromQuery] int page, [FromQuery] int pageSize)
    {
        var admins = await AdminService.GetAdmins(page, pageSize);
        
        return Ok(new {
            users = admins.Item1,
            count = admins.Item2,
        });
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetAdmin([FromRoute] Guid id)
    {
        var admin = await AdminService.GetAdminById(id);
        var userDto = UserDto.FromUser(admin);
        
        return Ok(userDto);
    }

    [HttpPut]
    [Route("{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdateAdmin([FromRoute] Guid id, [FromBody] UserDto userDto)
    {
        await AdminService.UpdateAdmin(id, userDto);
        
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