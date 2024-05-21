using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Application.Services;

public interface ISignInService
{
    public string GenerateJwToken(User user);
    public string? HashPassword(string password);
    public bool CheckPasswordHash(string passwordHash, string password);
}
