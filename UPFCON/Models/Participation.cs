using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Participation
{
    [Key] public Guid AttendeeId { get; set; }
    [ForeignKey(nameof(AttendeeId))] public Attendee Attendee { get; set; } = null!;
    
    [Key] public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId))] public Event Event { get; set; } = null!;
    
    [Required, DefaultValue(false)] public bool IsExpert { get; set; }
}