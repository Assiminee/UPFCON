namespace UPFCON.Requests;

public class SubmissionRulesDto
{
    public string Font             { get; set; } = string.Empty;
    public int    MinPages         { get; set; }
    public int    MaxPages         { get; set; }
    public string Formats          { get; set; } = string.Empty;
    public int    Margins          { get; set; }
    public int    LineSpacing      { get; set; }
    public string AdditionalRules  { get; set; } = string.Empty;
    public string FileNameFormat   { get; set; } = string.Empty;
    public DateTime SubmissionDeadline { get; set; }
}
