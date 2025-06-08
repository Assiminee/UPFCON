using System.ComponentModel.DataAnnotations;

namespace UPFCON.Requests;

public class AdminDto
{
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
        
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public DateTime Birthdate { get; set; }
    
    [Required]
    public string Address { get; set; } = string.Empty;

    // public static UserProfileDto FromUser(User user)
    // {
    //     var profileDto = new UserProfileDto()
    //     {
    //         Description = user.Description ?? string.Empty,
    //         FirstName = user.FirstName,
    //         LastName = user.LastName,
    //         Email = user.Email ?? string.Empty,
    //         PhoneNumber = user.PhoneNumber ?? string.Empty,
    //         Birthdate = user.Birthdate,
    //         Address = user.Address ?? string.Empty,
    //     };
    //
    //     if (user.Author != null)
    //         profileDto.Expertise = user.Author.Expertise;
    //
    //     return profileDto;
    // }
}