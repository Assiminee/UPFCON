using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;
public enum EventValidationStatus  // Enum recommandée = moins d’erreurs de frappe
{
    Pending,        // 0 – aucune décision
    InReview,       // 1 – Dean ou VP a donné un avis
    Validated,      // 2 – Les trois ont « Approved »
    Rejected        // 3 – Au moins un « Rejected » / « ToBeRevised »
}

public class Event
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(100)] public required string Title { get; set; }
    
    [Required, MaxLength(10)] public required string Acronym { get; set; }
    
    [Required, MaxLength(255)] public required string EventType { get; set; }
    
    [Required] public DateTime StartsAt { get; set; }
    
    [Required] public DateTime EndsAt { get; set; }
    
    [Required, MaxLength(100)] public required string Theme { get; set; }
    
    [Required, MaxLength(2048)] public required string Location { get; set; }
    
    [Required, MaxLength(2048)] public required string Topics { get; set; }
    
    [Required, MaxLength(2048)] public required string SubTopics { get; set; }
    
    [Required, MaxLength(2048)] public required string Logo { get; set; }
    
    [Required, MaxLength(255)] public required string Description { get; set; }
    
    public IList<Attendance> Attendees { get; set; } = new List<Attendance>();
    public IList<Paper> SubmittedPapers { get; set; } = new List<Paper>();
    public IList<CommitteeMember> OrganizingCommittee { get; set; } = new List<CommitteeMember>();

    public SubmissionRules? SubmissionRules { get; set; }
    
    public IList<BoardDirectorDecision> BoardDecisions { get; set; } = new List<BoardDirectorDecision>();
    
    public EventValidationStatus ValidationStatus { get; set; } = EventValidationStatus.Pending;
    
    
    public IList<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
