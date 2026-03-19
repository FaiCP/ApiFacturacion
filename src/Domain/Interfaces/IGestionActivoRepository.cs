using Domain.Entities;

namespace Domain.Interfaces;

public interface IGestionActivoRepository : IGenericRepository<GestionActivo>
{
    Task<IReadOnlyList<GestionActivo>> SearchAsync(string searchTerm, int page, int pageSize);
    Task<int> CountSearchAsync(string searchTerm);
    Task<IReadOnlyList<GestionActivo>> GetHistorialAsync(string searchTerm, int page, int pageSize, int idCustodio);
    Task<int> CountHistorialAsync(string searchTerm, int idCustodio);
    Task<IReadOnlyList<GestionActivo>> GetAllActiveWithDetailsAsync();
    Task<IReadOnlyList<GestionActivo>> GetByIdsAsync(List<long> ids, bool borrado = false);
}
