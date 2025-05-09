namespace UPFCON.Models;

public enum BoardMemberRole
{
    President,
    VicePresident,
    Dean
}

public class BoardDirector : User
{
    public BoardDirector(
        string fname, string lname, string email, string phone,
        DateTime birthdate, string description, string address,
        string pwd, AccountStatus accountStatus, BoardMemberRole role)
        : base(fname, lname, email, phone, birthdate, description, address, pwd, accountStatus)
    {
        Role = role;
    }

    public BoardMemberRole Role { get; set; }
    public IList<BoardDirectorDecision> Decisions { get; set; }
}