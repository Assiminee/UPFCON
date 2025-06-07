namespace UPFCON.Requests;

public class EventDto
{
    public string Title { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Topics { get; set; } = string.Empty;
    public string SubTopics { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}