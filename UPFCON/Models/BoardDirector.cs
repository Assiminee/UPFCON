using System.ComponentModel.DataAnnotations;

namespace UPFCON.Models;

public enum BoardDirectorRole
{
    President,
    VicePresident,
    Dean
}

public class BoardDirector : User
{
    [Required] public required string Role { get; set; }
    
    public bool PasswordChanged { get; set; } = false;
    public IList<BoardDirectorDecision> Decisions { get; set; } = new List<BoardDirectorDecision>();
}