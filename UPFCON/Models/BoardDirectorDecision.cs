using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public enum ApprovalStatusEnum
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

    public string ApprovalStatus { get; set; } = Enum.GetName(ApprovalStatusEnum.PendingDecision) ?? string.Empty;

    [Required, MaxLength(255)] public string Comment { get; set; } = null!;
}
