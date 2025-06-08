using Microsoft.AspNetCore.Identity;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Requests;

namespace UPFCON.Services;

public class AdminService(UserManager<User> userManager, IUtils utils, UpfconContext context) : IAdminService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IUtils Utils { get; } = utils;
    private UpfconContext Context { get; } = context;

    public async Task<Admin> CreateAdmin(AdminDto adminDto)
    {
        var admin = new Admin
        {
            FirstName = adminDto.FirstName,
            LastName = adminDto.LastName,
            Email = adminDto.Email,
            PhoneNumber = adminDto.PhoneNumber,
            Birthdate = adminDto.Birthdate,
            Address = adminDto.Address,
            UserName = adminDto.Email,
        };
        var password = Utils.GenerateRandomPassword();

        var res = await UserManager.CreateAsync(admin, password);
        
        utils.LogErrors(res, "Failed to create admin");
        
        if (!res.Succeeded)
        {
            var exception = res.Errors.Any(e => 
                e.Code.Equals("DuplicateUserName") || e.Code.Equals("DuplicateEmail")
            );

            if (exception)
                throw new DuplicateEmailException($"The email {adminDto.Email} already exists.");
        }

        var roleRes = await UserManager.AddToRoleAsync(admin, Enum.GetName(Roles.Admin) ?? "");
        
        utils.LogErrors(roleRes, "Failed to create admin");
        
        if (!roleRes.Succeeded)
            throw new Exception("Failed to create admin");
        
        return admin;
    }

    public async Task<Admin> GetAdminById(Guid id)
    {
        var admin = await Context.Admins.FindAsync(id);
        
        if (admin == null)
            throw new NotFoundException("Admin not found");

        return admin;
    }
}