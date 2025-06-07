using System.ComponentModel.DataAnnotations;

namespace UPFCON.Models.DTOs;

public class RegistrationDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    public DateTime Birthdate { get; set; }
    
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public IList<string> Roles { get; set; } = new List<string>();
    
    [Required]
    public IList<DiplomaDto> Diplomas { get; set; } = new List<DiplomaDto>();
    
    public string? Expertise { get; set; }
    public bool? IsInternal { get; set; }
}