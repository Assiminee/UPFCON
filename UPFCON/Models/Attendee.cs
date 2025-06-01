using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Attendee
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public IList<Attendance> EventsAttended { get; set; } = new List<Attendance>();
}