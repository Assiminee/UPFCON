using System.ComponentModel.DataAnnotations;
using UPFCON.Models;

namespace UPFCON.Requests;

public class UserProfileDto
{
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public DateTime Birthdate { get; set; }
    
    [Required]
    public string Address { get; set; } = string.Empty;
    
    public string? Expertise { get; set; }

    public static UserProfileDto FromUser(User user)
    {
        var profileDto = new UserProfileDto()
        {
            Description = user.Description ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Birthdate = user.Birthdate,
            Address = user.Address ?? string.Empty,
        };

        if (user.Author != null)
            profileDto.Expertise = user.Author.Expertise;

        return profileDto;
    }
}