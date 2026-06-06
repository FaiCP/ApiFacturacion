using Domain.Entities;

namespace Domain.Interfaces;

public interface IClienteRepository : IGenericRepository<Cliente>
{
    Task<Cliente?> GetByIdentificacionAsync(string numeroIdentificacion);
    Task<(IReadOnlyList<Cliente> Items, int Total)> GetPagedAsync(int pagina, int cantidad, string? busqueda);
}
