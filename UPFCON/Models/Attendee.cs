using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Attendee
{
    [Key] public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId)), Required] public User User { get; set; } = null!;
    
    public IList<Participation> Events { get; set; } = new List<Participation>();
}