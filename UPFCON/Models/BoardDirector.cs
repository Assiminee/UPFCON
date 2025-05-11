namespace UPFCON.Models;

public enum BoardDirectorRole
{
    President,
    VicePresident,
    Dean
}

public class BoardDirector : User
{
    public required BoardDirectorRole Role { get; set; }
    public IList<BoardDirectorDecision> Decisions { get; set; } = new List<BoardDirectorDecision>();
}