using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public enum PaperStatus {
    PendingEvaluation,
    Accepted,
    Reject,
    RequiresEdits
}
public class Paper
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(100)] public required string Title { get; set; }
    
    [Required, MaxLength(4096)] public required string Abstract { get; set; }
    
    [Required] public DateTime PublicationDate { get; set; }
    
    [Required] public DateTime SubmittedAt { get; set; }
    
    [Required, MaxLength(2048)] public required string Path {get; set;}
    
    [Required, MaxLength(255)] public required string Keywords {get; set;}
    
    public required PaperStatus Status { get; set; }
    public IList<Contribution> Contributors { get; set; } = new List<Contribution>();
    public IList<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    
    public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId)), Required] public Event Event { get; set; } = null!;
    public TimeSlot? TimeSlot { get; set; }
}