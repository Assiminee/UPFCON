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
    [Key] public Guid AuthorId { get; set; }
    [ForeignKey(nameof(AuthorId))] public Author Author { get; set; } = null!;
    
    [Key] public Guid PaperId { get; set; }
    [ForeignKey(nameof(PaperId))] public Paper Paper { get; set; } = null!;
    
    [Required, DefaultValue(ContributorRole.Contributor)] public ContributorRole Role { get; set; }
}