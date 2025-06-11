using UPFCON.Models;

namespace UPFCON.Requests;

public class CommitteeMemberDto
{
    public Guid ChairmanId { get; set; }
    public CommitteeMemberRole Role { get; set; } = CommitteeMemberRole.Evaluator;
}
