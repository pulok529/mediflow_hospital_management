using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mediflow.Application.Abstractions.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Mediflow.Infrastructure.Auth;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string CreateToken(Guid userId, string email, IReadOnlyCollection<string> permissions, IReadOnlyCollection<string> roles)
    {
        var secret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email)
        };

        claims.AddRange(permissions.Select(p => new Claim("permission", p)));
        claims.AddRange(roles.Select(r => new Claim("role", r)));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

