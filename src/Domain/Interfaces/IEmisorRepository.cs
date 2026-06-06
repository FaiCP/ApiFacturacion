using Domain.Entities;

namespace Domain.Interfaces;

public interface IEmisorRepository : IGenericRepository<Emisor>
{
    Task<Emisor?> GetActivoAsync();
}
