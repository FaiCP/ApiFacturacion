using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FacturaRepository : GenericRepository<Factura>, IFacturaRepository
{
    public FacturaRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Factura?> GetByClaveAccesoAsync(string claveAcceso) =>
        await _dbSet.AsNoTracking()
            .Include(f => f.Cliente)
            .Include(f => f.Emisor)
            .FirstOrDefaultAsync(f => f.ClaveAcceso == claveAcceso);

    public async Task<Factura?> GetWithDetallesAsync(long id) =>
        await _dbSet.AsNoTracking()
            .Include(f => f.Cliente)
            .Include(f => f.Emisor)
            .Include(f => f.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<(IReadOnlyList<Factura> Items, int Total)> GetPagedAsync(
        int pagina, int cantidad, EstadoSRI? estado, DateTime? desde, DateTime? hasta, long? clienteId)
    {
        var page = pagina <= 0 ? 1 : pagina;
        IQueryable<Factura> query = _dbSet.AsNoTracking()
            .Include(f => f.Cliente)
            .Where(f => f.Borrado != true);

        if (estado.HasValue) query = query.Where(f => f.Estado == estado.Value);
        if (desde.HasValue)  query = query.Where(f => f.FechaEmision >= desde.Value);
        if (hasta.HasValue)  query = query.Where(f => f.FechaEmision <= hasta.Value);
        if (clienteId.HasValue) query = query.Where(f => f.ClienteId == clienteId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(f => f.FechaEmision)
            .Skip((page - 1) * cantidad)
            .Take(cantidad)
            .ToListAsync();

        return (items, total);
    }

    public async Task<int> GetUltimoSecuencialAsync(long emisorId, string serie)
    {
        var ultimo = await _dbSet
            .Where(f => f.EmisorId == emisorId && f.Serie == serie)
            .OrderByDescending(f => f.Id)
            .Select(f => f.Secuencial)
            .FirstOrDefaultAsync();

        return ultimo != null && int.TryParse(ultimo, out var num) ? num : 0;
    }
}
