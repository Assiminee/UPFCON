using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UPFCON.Models;

public enum AccountStatus
{
    Verified,
    PendingVerification,
    Rejected,
    Deleted
}

public class User
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(100)] public required string Fname { get; set; }
    
    [Required, MaxLength(100)] public required string Lname { get; set; }
    
    [Required, MaxLength(255), EmailAddress] public required string Email { get; set; }
    
    [Required, MaxLength(13), Phone] public required string Phone { get; set; }
    
    [Required] public DateTime Birthdate { get; set; }
    
    [MaxLength(255)] public required string Description { get; set; }
    
    [Required, MaxLength(255)] public required string Address { get; set; }
    
    [Required, MaxLength(255)] public required string Pwd { get; set; }
    
    public AccountStatus AccountStatus { get; set; }
    
    public IList<Diploma> Diplomas { get; set; } = new List<Diploma>();
    
    public Author? Author { get; set; }
    public Attendee? Attendee { get; set; }
    public Chairman? Chairman { get; set; }

    [NotMapped] public string FullName
    {
        get => $"{Fname} {Lname}";
    }
}