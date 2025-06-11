using System.ComponentModel.DataAnnotations;
using UPFCON.Models;

namespace UPFCON.Requests;

public class UserDto
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

    public static UserDto FromUser(User user)
    {
        return new UserDto
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

    public static User SetAdminFromDto(User user, UserDto dto)
    {
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Birthdate = dto.Birthdate;
        user.Address = dto.Address;
        user.AccountStatus = dto.AccountStatus;
        
        return user;
    }
}