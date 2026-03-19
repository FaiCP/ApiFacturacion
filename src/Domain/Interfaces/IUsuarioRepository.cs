namespace Domain.Interfaces;

/// <summary>
/// Interfaz de repositorio para Usuarios
/// </summary>
public interface IUsuarioRepository : IGenericRepository<Entities.Usuario>
{
    Task<Entities.Usuario?> GetByEmailAsync(string email);
    Task<Entities.Usuario?> GetByNombreAsync(string nombre);
    Task<bool> ValidateCredentialsAsync(string nombre, string password);
}
