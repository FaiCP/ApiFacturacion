using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implementación de repositorio para Usuarios
/// </summary>
public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioRepository(ApplicationDbContext context, IPasswordHasher passwordHasher) : base(context)
    {
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> GetByNombreAsync(string nombre)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Nombre == nombre);
    }

    public async Task<bool> ValidateCredentialsAsync(string nombre, string password)
    {
        var usuario = await _dbSet
            .FirstOrDefaultAsync(u => u.Nombre == nombre);

        if (usuario == null)
            return false;

        return _passwordHasher.VerifyPassword(password, usuario.Password);
    }
}
