using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Chairman
{
    [Key] public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User User { get; set; } = null!;
    
    [Required] public bool IsInternal { get; set; }
    
    public IList<CommitteeMember> Memberships { get; set; } = new List<CommitteeMember>();
}
