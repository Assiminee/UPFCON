namespace UPFCON.Authorization;

public class JwtToken
{
    public string? AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}