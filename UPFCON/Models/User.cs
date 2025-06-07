using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace UPFCON.Models;

public enum AccountStatus
{
    Verified,
    PendingVerification,
    Rejected,
    Deleted
}

public enum Roles
{
    Admin,
    Author,
    Chairman,
    Attendee,
    BoardDirector
}

public class User : IdentityUser<Guid>
{
    // ─── Removed the following parameters ───────────────────────────────────────────────────
    // public Guid Id { get; set; }
    // [Required, MaxLength(255), EmailAddress] public required string Email { get; set; }
    // [Required, MaxLength(255)] public required string Pwd { get; set; }
    // Since IdentityUser already has these parameters defined
    // ────────────────────────────────────────────────────────────────────────────────────────

    [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    
    [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
    
    [Required] public DateTime Birthdate { get; set; }
    
    [MaxLength(255)] public string? Description { get; set; }
    
    [Required, MaxLength(255)] public string? Address { get; set; }

    public string AccountStatus { get; set; } = Enum.GetName(Models.AccountStatus.PendingVerification) ?? string.Empty;
    
    public IList<Diploma> Diplomas { get; set; } = new List<Diploma>();
    
    public Author? Author { get; set; }
    public Attendee? Attendee { get; set; }
    public Chairman? Chairman { get; set; }

    [NotMapped] public string FullName
    {
        get => $"{FirstName} {LastName}";
    }
}