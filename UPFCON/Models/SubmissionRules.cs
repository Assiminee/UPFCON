using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class SubmissionRules
{
    [Key] public Guid EventId { get; set; }
    [ForeignKey((nameof(EventId)))] public Event Event { get; set; } = null!;
    
    [Required, MaxLength(50)] public required string Font { get; set; }
    
    [Required] public int MinPages { get; set; }
    
    [Required] public int MaxPages { get; set; }
    
    [Required, MaxLength(50)] public required string Formats { get; set; }
    
    [Required] public int Margins { get; set; }
    
    [Required] public int LineSpacing { get; set; }
    
    [Required, MaxLength(50)] public required string AdditionalRules { get; set; }
    
    [Required, MaxLength(50)] public required string FileNameFormat { get; set; }
    
    [Required] public DateTime SubmissionDeadline { get; set; }
}