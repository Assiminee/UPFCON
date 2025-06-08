using UPFCON.Models;
using UPFCON.Requests;

namespace UPFCON.Interfaces;

public interface IAdminService
{
    Task<Admin> CreateAdmin(AdminDto adminDto);
    Task<Admin> GetAdminById(Guid id);
}