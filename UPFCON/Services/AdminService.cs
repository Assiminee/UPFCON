using Microsoft.AspNetCore.Identity;
using UPFCON.Exceptions;
using UPFCON.Interfaces;
using UPFCON.Models;
using UPFCON.Models.Context;
using UPFCON.Requests;

namespace UPFCON.Services;

public class AdminService(UserManager<User> userManager, IUtils utils, UpfconContext context, IGenericService genericService) : IAdminService
{
    private UserManager<User> UserManager { get; } = userManager;
    private IUtils Utils { get; } = utils;
    private UpfconContext Context { get; } = context;
    public IGenericService GenericService { get; } = genericService;

    public async Task<Admin> CreateAdmin(UserDto userDto)
    {
        var admin = new Admin
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber,
            Birthdate = userDto.Birthdate,
            Address = userDto.Address,
            UserName = userDto.Email,
            AccountStatus = userDto.AccountStatus,
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
                throw new DuplicateEmailException($"The email {userDto.Email} already exists.");
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

    public async Task<(IList<UserDto>, int)> GetAdmins(int page, int pageSize)
    {
        var userDtos = new List<UserDto>();
        
        var result = await GenericService.GetPagedResultAsync(
            Context.Admins.AsQueryable(), page, pageSize,
            a => a.FirstName);

        if (result.Count == 0)
            return ( [], 0 );
        
        foreach (var admin in result.Items)
            userDtos.Add(UserDto.FromUser(admin));

        return (userDtos, result.Count);
    }

    public async Task UpdateAdmin(Guid id, UserDto userDto)
    {
        var admin = await UserManager.FindByIdAsync(id.ToString());
        
        if (admin == null)
            throw new NotFoundException("Admin not found");
        
        admin = UserDto.SetUserFromDto(admin, userDto);
        
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