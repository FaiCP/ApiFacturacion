using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
{
    public ProductoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Producto?> GetByCodigoAsync(string codigoPrincipal) =>
        await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(p => p.CodigoPrincipal == codigoPrincipal && p.Borrado != true);

    public async Task<(IReadOnlyList<Producto> Items, int Total)> GetPagedAsync(int pagina, int cantidad, string? busqueda)
    {
        var page = pagina <= 0 ? 1 : pagina;
        IQueryable<Producto> query = _dbSet.AsNoTracking().Where(p => p.Borrado != true);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var term = busqueda.ToLower();
            query = query.Where(p =>
                p.Descripcion.ToLower().Contains(term) ||
                p.CodigoPrincipal.ToLower().Contains(term));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Descripcion)
            .Skip((page - 1) * cantidad)
            .Take(cantidad)
            .ToListAsync();

        return (items, total);
    }
}
