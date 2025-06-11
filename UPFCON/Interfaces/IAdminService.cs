using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Interfaces;

public interface IAdminService
{
    Task<Admin> CreateAdmin(AdminDto adminDto);
    Task<Admin> GetAdminById(Guid id);
    IList<AdminDto> GetAdmins(int page, int pageSize);
    int GetCount();
    Task UpdateAdmin(Guid id, AdminDto adminDto);
    Task DeleteAdmin(Guid id);
}