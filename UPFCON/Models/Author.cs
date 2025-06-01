using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Author
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required, MaxLength(255)] public required string Expertise { get; set; }
    public IList<Contribution> Contributions { get; set; } = new List<Contribution>();
}