using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public enum DiplomaVerificationStatus
{
    Verified,
    PendingVerification,
    Rejected
}

public class Diploma
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required, MaxLength(255)] public required string Title { get; set; }
    
    [Required] public DateTime IssueDate { get; set; }
    
    [Required, MaxLength(255)] public required string Path { get; set; }
    public DiplomaVerificationStatus VerificationStatus { get; set; }
    public Guid? AdminId { get; set; }
    public Admin? VerifiedBy { get; set; }
    
    public DateTime? VerifiedAt { get; set; }
}
