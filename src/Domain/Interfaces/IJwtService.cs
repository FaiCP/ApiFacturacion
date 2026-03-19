using Domain.Entities;

namespace Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(Usuario usuario);
    (bool IsValid, string? Nombre) ValidateToken(string token);
}
