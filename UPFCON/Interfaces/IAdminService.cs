using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Interfaces;

public interface IAdminService
{
    Task<Admin> CreateAdmin(UserDto userDto);
    Task<Admin> GetAdminById(Guid id);
    Task<(IList<UserDto>, int)> GetAdmins(int page, int pageSize);
    int GetCount();
    Task UpdateAdmin(Guid id, UserDto userDto);
    Task DeleteAdmin(Guid id);
}