using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public enum CommitteeMemberRole
{
    HeadChairman,
    Evaluator,
    ExternalOrganizerChairman
}

public enum InvitationStatus
{
    Accepted,
    PendingResponse,
    Rejected
}

public class CommitteeMember
{
    [Key] public Guid ChairmanId { get; set; }
    [ForeignKey(nameof(ChairmanId))] public Chairman Chairman { get; set; } = null!;
    [Key] public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId))] public Event Event {get; set;}  = null!;
    
    [Required, DefaultValue(CommitteeMemberRole.Evaluator)] public CommitteeMemberRole Role {get; set;}
    
    [Required] public DateTime InvitedAt { get; set; }
    
    public DateTime? RespondedAt { get; set; }
    
    [Required, DefaultValue(InvitationStatus.PendingResponse)] public InvitationStatus InvitationStatus { get; set; }
    
    public IList<Evaluation>? Evaluations { get; set; }
}
