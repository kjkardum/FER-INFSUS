using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FER.InfSus.Time.Application.Configuration;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Domain.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace FER.InfSus.Time.Infrastructure.Identity;

public class SignInService : ISignInService
{
    private readonly JwtConfiguration _jwtConfiguration;

    public SignInService(IOptions<JwtConfiguration> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration.Value;
    }

    public string GenerateJwToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("tenant", user.TenantId.ToString()),
            new Claim("role", user.UserType.ToString())
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfiguration.DurationInMinutes),
                signingCredentials: signingCredentials));
    }

    public string? HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool CheckPasswordHash(string passwordHash, string password) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
