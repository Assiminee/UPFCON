namespace UPFCON.Models;

public enum BoardMemberRole
{
    President,
    VicePresident,
    Dean
}

public class BoardDirector : User
{
    public BoardMemberRole Role { get; set; }
    public IList<BoardDirectorDecision> Decisions { get; set; }
}