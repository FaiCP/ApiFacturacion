namespace Domain.Interfaces;

/// <summary>
/// Interfaz de repositorio para Custodios
/// </summary>
public interface ICustodioRepository : IGenericRepository<Entities.Custodio>
{
    Task<IReadOnlyList<Entities.Custodio>> GetByDepartamentoAsync(long departamentoId);
    Task<Entities.Custodio?> GetByCedulaAsync(string cedula);
    Task<IReadOnlyList<Entities.Custodio>> SearchAsync(string searchTerm, int page, int pageSize);
    Task<int> CountSearchAsync(string searchTerm);
}
