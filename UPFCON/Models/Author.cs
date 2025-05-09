using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Author
{
    [Key] public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))] public User User { get; set; } = null!;
    
    [Required, MaxLength(255)] public string Expertise { get; set; } = null!;
    
    public IList<Contribution> Contributions { get; set; } = new List<Contribution>();
}