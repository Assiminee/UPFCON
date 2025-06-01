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
    public Guid ChairmanId { get; set; }
    public Chairman Chairman { get; set; } = null!;
    public Guid EventId { get; set; }
    public Event Event {get; set;}  = null!;
    public CommitteeMemberRole Role {get; set;}
    
    [Required] public DateTime InvitedAt { get; set; }
    
    public DateTime? RespondedAt { get; set; }
    public InvitationStatus InvitationStatus { get; set; }
    public IList<Evaluation>? Evaluations { get; set; }
}
