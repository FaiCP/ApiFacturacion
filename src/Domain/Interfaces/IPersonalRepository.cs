using Domain.Entities;

namespace Domain.Interfaces;

public interface IPersonalRepository : IGenericRepository<Persona>
{
    Task<IReadOnlyList<Persona>> SearchAsync(string searchTerm, int page, int pageSize);
    Task<int> CountSearchAsync(string searchTerm);
    Task<IReadOnlyList<Persona>> GetByIdsAsync(List<long> ids);
    Task<IReadOnlyList<Persona>> GetAllActiveAsync();
}
