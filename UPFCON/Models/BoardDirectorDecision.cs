using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public enum ApprovalStatus
{
    Approved,
    Rejected,
    ToBeRevised,
    PendingDecision
}

public class BoardDirectorDecision
{
    [Key] public Guid BoardDirectorId { get; set; }
    [Key] public Guid EventId { get; set; }
    
    [ForeignKey(nameof(BoardDirectorId))] public BoardDirector BoardDirector { get; set; } = null!;
    
    [ForeignKey(nameof(EventId))] public Event Event { get; set; } = null!;
    
    [Required, DefaultValue(ApprovalStatus.PendingDecision)] public ApprovalStatus ApprovalStatus { get; set; }

    [Required, MaxLength(255)] public string Comment { get; set; } = null!;
}
