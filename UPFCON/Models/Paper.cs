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
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public Guid Id { get; set; }
    
    [Required, MaxLength(100)] public string Title { get; set; } = null!;
    
    [Required, MaxLength(255)] public string Abstract { get; set; } = null!;
    
    [Required] public DateTime PublicationDate { get; set; }
    
    [Required] public DateTime SubmittedAt { get; set; }
    
    [Required] public string Path {get; set;} = null!;
    
    [Required] public string Keywords {get; set;} = null!;
    
    [Required, DefaultValue(PaperStatus.PendingEvaluation)] public PaperStatus Status { get; set; }
    
    public IList<Contribution> Contributors { get; set; } = new List<Contribution>();
    public IList<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    
    public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId))] public Event Event { get; set; } = null!;
}