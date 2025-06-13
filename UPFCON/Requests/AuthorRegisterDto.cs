using System.ComponentModel.DataAnnotations;

namespace UPFCON.Requests;

public class AuthorRegisterDto
{
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Expertise { get; set; } = string.Empty;

    public AuthorRegisterDto()
    {
    }

    public AuthorRegisterDto(string firstName, string lastName, string email, string expertise)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Expertise = expertise;
    }
}