using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public enum TimeSlotType
{
    Paper,
    Break, 
    ArtisticBreak,
    Speech,
    Deliberations,
    CommitteeComments
}

public class TimeSlot
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    
    public Guid PaperId { get; set; }
    public Paper Paper { get; set; } = null!;
    
    [Required] public DateTime Start { get; set; }
    
    [Required] public DateTime End { get; set; }
    
    [Required, MaxLength(100)] public required string Title { get; set; }
    
    [Required, MaxLength(255)] public required string Description { get; set; }
    
    [Required] public TimeSlotType Type { get; set; }
}