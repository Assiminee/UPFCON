using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UPFCON.Models;

public class User
{
    public User(string fname, string lname, string email, string phone, DateTime birthdate, string description, string address, string pwd, AccountStatus accountStatus)
    {
        Fname = fname;
        Lname = lname;
        Email = email;
        Phone = phone;
        Birthdate = birthdate;
        Description = description;
        Address = address;
        Pwd = pwd;
        AccountStatus = accountStatus;
    }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public Guid Id { get; set; }
    
    [Required, MaxLength(50)] public string Fname { get; set; }
    
    [Required, MaxLength(50)] public string Lname { get; set; }
    
    [Required, MaxLength(255), Index(IsUnique=true)] public string Email { get; set; }
    
    [Required, MaxLength(13), Index(IsUnique=true)] public string Phone { get; set; }
    
    [Required] public DateTime Birthdate { get; set; }
    
    [MaxLength(255)] public string Description { get; set; }
    
    [Required, MaxLength(255)] public string Address { get; set; }
    
    [Required, MaxLength(255)] public string Pwd { get; set; }
    
    [Required, DefaultValue(AccountStatus.PendingVerification)] public AccountStatus AccountStatus { get; set; }
    
    [Required] public IList<Diploma> Diplomas { get; set; }
    
    public Author? Author { get; set; }
    public Attendee? Attendee { get; set; }
    public Chairman? Chairman { get; set; }

    [NotMapped] public string FullName
    {
        get => $"{Fname} {Lname}";
    }

    public override string ToString()
    {
        var author = Author != null ? Id.ToString() : "Not an author";
        var attendee = Attendee != null ? Id.ToString() : "Not an attendee";
        var chairman = Chairman != null ? Id.ToString() : "Not an chairman";
        var diplomas = new StringBuilder();

        if (Diplomas.Count > 0)
        {
            diplomas.AppendLine("[");
            for (var i = 0; i < Diplomas.Count; i++)
            {
                diplomas.Append($"\t{Diplomas[i]}");
                if (i < Diplomas.Count - 1)
                    diplomas.Append(", \n");
            }
            diplomas.Append(']');
        }

        return $"User [ Id = {Id}\n" +
               $"\tFname = {Fname}\n" +
               $"\tLname = {Lname}\n" +
               $"\tEmail = {Email}\n" +
               $"\tPhone = {Phone}\n" +
               $"\tBirthdate = {Birthdate} \n" +
               $"\tDescription = {Description}\n" +
               $"\tAddress = {Address}\n" +
               $"\tPwd = {Pwd}\n" +
               $"\tAccountStatus = {AccountStatus.ToString()}\n" +
               $"\tAuthorId = {author} \n" +
               $"\tAttendeeId = {attendee} \n" +
               $"\tChairmanId = {chairman} \n" +
               $"\tDiplomas = {diplomas} ]";
    }
}