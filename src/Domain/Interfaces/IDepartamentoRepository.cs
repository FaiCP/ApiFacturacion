using Domain.Entities;

namespace Domain.Interfaces;

public interface IDepartamentoRepository : IGenericRepository<Departamento>
{
    Task<IReadOnlyList<Departamento>> SearchAsync(string searchTerm, int page, int pageSize);
    Task<int> CountSearchAsync(string searchTerm);
}
