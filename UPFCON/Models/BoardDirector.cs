namespace UPFCON.Models;

public enum BoardDirectorRole
{
    President,
    VicePresident,
    Dean
}

public class BoardDirector : User
{
   
  //  [CheckConstraint("CK_AllowedBoardDirectorRole", "[Role] IN ('President', 'VicePresident', 'Dean')")]
    public required BoardDirectorRole Role { get; set; }
    public IList<BoardDirectorDecision> Decisions { get; set; } = new List<BoardDirectorDecision>();
}