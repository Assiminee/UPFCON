using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Attendance
{
    public Guid AttendeeId { get; set; }
    public Attendee Attendee { get; set; } = null!;
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public bool IsExpert { get; set; }
}