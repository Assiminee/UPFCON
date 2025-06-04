using System.ComponentModel.DataAnnotations;

namespace UPFCON.Models.DTOs;

public class DiplomaDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public DateTime IssueDate { get; set; }
    
    [Required]
    public IFormFile DiplomaFile { get; set; } = null!;
}