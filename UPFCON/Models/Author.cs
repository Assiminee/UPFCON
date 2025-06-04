using System.ComponentModel.DataAnnotations;

namespace UPFCON.Models;

public class Author
{
    public Author() { }

    public Author(string expertise)
    {
        Expertise = expertise;
    }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required, MaxLength(255)] public string Expertise { get; set; } = string.Empty;
    public IList<Contribution> Contributions { get; set; } = new List<Contribution>();
}