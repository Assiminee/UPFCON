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
            AccountStatus = adminDto.AccountStatus,
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

    public int GetCount()
    {
        return Context.Admins.Count();
    }

    public IList<AdminDto> GetAdmins(int page, int pageSize)
    {
        var admins = Context.Admins
            .OrderBy(a => a.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        if (admins.Count == 0)
            return [];

        var adminDtos = new List<AdminDto>();
        
        foreach (var admin in admins)
            adminDtos.Add(AdminDto.FromAdmin(admin));

        return adminDtos;
    }

    public async Task UpdateAdmin(Guid id, AdminDto adminDto)
    {
        var admin = await GetAdminById(id);
        admin = AdminDto.SetAdminFromDto(admin, adminDto);
        
        var res = await UserManager.UpdateAsync(admin);
        
        utils.LogErrors(res, "Failed to update admin");
        
        if (!res.Succeeded)
            throw new Exception("Failed to update admin");
    }

    public async Task DeleteAdmin(Guid id)
    {
        var admin = await GetAdminById(id);
        await Context.Entry(admin).Collection(u => u.VerifiedDiplomas).LoadAsync();

        if (admin.VerifiedDiplomas.Count == 0)
        {
            var res = await UserManager.DeleteAsync(admin);
            utils.LogErrors(res, "Failed to delete admin");
            
            if (!res.Succeeded)
                throw new Exception("Failed to delete admin");
            
            return;
        }
        Utils.LogInformation($"DIPLOMA {admin.VerifiedDiplomas[0].Title}");

        admin.AccountStatus = Enum.GetName(AccountStatus.Deleted) ?? "Deleted";
        var updateRes = await UserManager.UpdateAsync(admin);
        
        utils.LogErrors(updateRes, "Failed to update admin");
        
        if (!updateRes.Succeeded)
            throw new Exception("Failed to update admin");
    }
}