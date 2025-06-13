using UPFCON.Models;

namespace UPFCON.Responses;

public class PaperResponseDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = "";
    
    public string Abstract { get; set; } = "";
    
    public string Path { get; set; } = "";
    
    public string Keywords { get; set; } = "";
    
    public DateTime PublicationDate { get; set; }
    
    public DateTime SubmittedAt { get; set; }
    
    public string Status { get; set; } = "";

    public Guid EventId { get; set; }
    
    public string EventName { get; set; } = "";

    public List<string> ContributorsNames { get; set; } = new();
    
    public IList<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

}