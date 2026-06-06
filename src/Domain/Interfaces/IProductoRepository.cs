using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductoRepository : IGenericRepository<Producto>
{
    Task<Producto?> GetByCodigoAsync(string codigoPrincipal);
    Task<(IReadOnlyList<Producto> Items, int Total)> GetPagedAsync(int pagina, int cantidad, string? busqueda);
}
