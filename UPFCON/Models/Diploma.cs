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
    public Diploma(string title, DateTime issueDate, string path, DiplomaVerificationStatus status)
    {
        Title = title;
        IssueDate = issueDate;
        Path = path;
        VerificationStatus = status;
    }

    [Key] public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User User { get; set; }
    
    [Required] public string Title { get; set; }
    
    [Required] public DateTime IssueDate { get; set; }
    
    [Required] public string Path { get; set; }
    
    [Required, DefaultValue(DiplomaVerificationStatus.PendingVerification)] public DiplomaVerificationStatus VerificationStatus { get; set; }
    
    public Guid? AdminId { get; set; }
    [ForeignKey(nameof(AdminId))] public Admin? VerifiedBy { get; set; }
    
    public DateTime? VerifiedAt { get; set; }

    public override string ToString()
    {
        var verifiedBy = VerifiedBy != null ? $"{{Id: {VerifiedBy.Id}, FullName: {VerifiedBy.FullName}}}" : "Null";
        var verifiedAt = VerifiedAt != null ? VerifiedAt.ToString() : "Null";
        
        return $"Diploma [ Id = {Id}\n" +
               $"\tTitle = {Title}\n" +
               $"\tIssueDate = {IssueDate}\n" +
               $"\tLocationOnDisk = {Path}\n" +
               $"\tVerificationStatus = {VerificationStatus.ToString()}\n" +
               $"\tVerifiedBy = {verifiedBy}\n" +
               $"\tVerifiedAt = {verifiedAt} ]";

    }
}
