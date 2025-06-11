using System.ComponentModel.DataAnnotations;
using UPFCON.Models;

namespace UPFCON.Requests;

public class AdminDto
{

    [Required] public string Id { get; set; } = string.Empty;
    
    [Required] public string FirstName { get; set; } = string.Empty;
    
    [Required] public string LastName { get; set; } = string.Empty;
        
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required] public DateTime Birthdate { get; set; }
    
    [Required] public string Address { get; set; } = string.Empty;
    [Required] public string AccountStatus { get; set; } = string.Empty;

    public static AdminDto FromAdmin(Admin user)
    {
        return new AdminDto
        {
            Id = user.Id.ToString(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Birthdate = user.Birthdate,
            Address = user.Address,
            AccountStatus = user.AccountStatus
        };
    }

    public static Admin SetAdminFromDto(Admin admin, AdminDto dto)
    {
        admin.FirstName = dto.FirstName;
        admin.LastName = dto.LastName;
        admin.PhoneNumber = dto.PhoneNumber;
        admin.Birthdate = dto.Birthdate;
        admin.Address = dto.Address;
        admin.AccountStatus = dto.AccountStatus;
        
        return admin;
    }
}