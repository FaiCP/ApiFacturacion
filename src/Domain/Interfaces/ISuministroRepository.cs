using Domain.Entities;

namespace Domain.Interfaces;

public interface ISuministroRepository : IGenericRepository<Suministro>
{
    Task<IReadOnlyList<Suministro>> SearchAsync(string searchTerm, int page, int pageSize);
    Task<int> CountSearchAsync(string searchTerm);
}
