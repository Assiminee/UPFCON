using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UPFCON.Interfaces;

namespace UPFCON.Services;

public class AuthService(IConfiguration configuration) : IAuth
{
    private IConfiguration Configuration { get; } = configuration;

    public string GenerateJwtToken(IEnumerable<Claim> claims, DateTime expiresAt)
    {
        var secretKey = Encoding.ASCII.GetBytes(Configuration["JWT:SecretKey"] ?? string.Empty);

        JwtSecurityToken jwtToken = new JwtSecurityToken(
            issuer: Configuration["JWT:Issuer"],
            audience: Configuration["JWT:Audience"],
            claims: claims,
            notBefore: DateTime.Now,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature
            )
        );
        
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    } 
}