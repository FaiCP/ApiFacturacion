using Domain.Entities;

namespace Domain.Interfaces;

public interface IKitRepository : IGenericRepository<Kit>
{
    Task<IReadOnlyList<Kit>> SearchAsync(string searchTerm, int page, int pageSize);
    Task<int> CountSearchAsync(string searchTerm);
    Task<IReadOnlyList<Kit>> GetAllActiveAsync();
}
