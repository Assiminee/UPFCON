using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UPFCON.Interfaces;
using UPFCON.Requests;

namespace UPFCON.Controllers;

[Authorize]
[Route("/api/v1/users")]
public class UserController(IUtils utils, IUserService userService) : Controller
{
    private IUtils Utils { get; } = utils;
    private IUserService UserService { get; } = userService;

    [HttpGet]
    [Route("profile")]
    public async Task<IActionResult> GetProfileAsync()
    {
        var user = await UserService.GetFromJwtEmailClaim(HttpContext);
        
        return Ok(UserProfileDto.FromUser(user));
    }

    [HttpPut]
    [Route("profile")]
    public async Task<IActionResult> EditProfileAsync([FromBody] UserProfileDto userProfileDto)
    {
        var user = await UserService.GetFromJwtEmailClaim(HttpContext);
        await UserService.EditUserAsync(user, userProfileDto);
        
        return Ok();
    }
    
    [HttpPut]
    [Route("profile/password")]
    public async Task<IActionResult> EditPasswordAsync([FromBody] ChangePasswordDto passwords)
    {
        var user = await UserService.GetFromJwtEmailClaim(HttpContext);
        await UserService.EditUserPasswordAsync(user, passwords);
        
        return Ok();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsersAsync([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = await UserService.GetUsersAsync(page, pageSize);
        
        return Ok(new
        {
            users = result.Item1,
            count = result.Item2
        });
    }
    
    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserAsync([FromRoute] string id)
    {
        Utils.LogInformation($"User id: {id}");
        var result = await UserService.FindUserById(id);
        var userDto = UserDto.FromUser(result);
        
        return Ok(userDto);
    }

    // [HttpDelete]
    // [Route("profile")]
    // public async Task<IActionResult> DeleteProfileAsync()
    // {
    //     var user = await UserService.GetFromJwtEmailClaim(HttpContext);
    //     
    // }
}