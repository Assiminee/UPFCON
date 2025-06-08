using System.ComponentModel.DataAnnotations;

namespace UPFCON.Requests;

public class AccountActivationPayload
{
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string ConfirmPassword { get; set; } = string.Empty;
    [Required] public string Token { get; set; } = string.Empty;
    
}