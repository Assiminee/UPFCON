using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Participation
{
    [Key] public Guid AttendeeId { get; set; }
    [ForeignKey(nameof(AttendeeId)), Required] public Attendee Attendee { get; set; } = null!;
    
    [Key] public Guid EventId { get; set; }
    [ForeignKey(nameof(EventId)), Required] public Event Event { get; set; } = null!;
    
    [Required, DefaultValue(false)] public bool IsExpert { get; set; }
}