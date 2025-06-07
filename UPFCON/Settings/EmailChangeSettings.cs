using System.ComponentModel.DataAnnotations;

namespace UPFCON.Settings;

public class EmailChangeSettings
{
    [Required] public string Subject { get; set; } = string.Empty;
    [Required] public string Body { get; set; } = string.Empty;
}