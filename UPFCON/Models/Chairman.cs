using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Chairman
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public required bool IsInternal { get; set; }
    public IList<CommitteeMember> Memberships { get; set; } = new List<CommitteeMember>();
}
