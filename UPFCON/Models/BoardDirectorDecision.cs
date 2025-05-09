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
    public Guid BoardDirectorId { get; set; }
    public Guid EventId { get; set; }
    public BoardDirector BoardDirector { get; set; } = null!;
    public Event Event { get; set; } = null!;
    
    [Required, DefaultValue(ApprovalStatus.PendingDecision)] public ApprovalStatus ApprovalStatus { get; set; }

    [Required, MaxLength(255)] public string Comment { get; set; } = null!;
}
