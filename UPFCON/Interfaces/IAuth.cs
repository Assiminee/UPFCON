using System.Security.Claims;

namespace UPFCON.Interfaces;

public interface IAuth
{
    string GenerateJwtToken(IEnumerable<Claim> claims, DateTime expiresAt);
}