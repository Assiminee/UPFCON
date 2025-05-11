using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public enum ContributorRole
{
    HeadAuthor,
    Contributor
}
public class Contribution
{
    public Guid AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Paper Paper { get; set; } = null!;
    public required ContributorRole Role { get; set; }
}