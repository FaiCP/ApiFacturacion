using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RetencionRepository : GenericRepository<Retencion>, IRetencionRepository
{
    public RetencionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Retencion?> GetWithDetallesAsync(long id) =>
        await _dbSet.AsNoTracking()
            .Include(r => r.SujetoRetenido)
            .Include(r => r.Emisor)
            .Include(r => r.Detalles)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<(IReadOnlyList<Retencion> Items, int Total)> GetPagedAsync(int pagina, int cantidad, EstadoSRI? estado, long? facturaId)
    {
        var page = pagina <= 0 ? 1 : pagina;
        IQueryable<Retencion> query = _dbSet.AsNoTracking()
            .Include(r => r.SujetoRetenido)
            .Where(r => r.Borrado != true);

        if (estado.HasValue) query = query.Where(r => r.Estado == estado.Value);
        if (facturaId.HasValue) query = query.Where(r => r.FacturaId == facturaId.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.FechaEmision).Skip((page - 1) * cantidad).Take(cantidad).ToListAsync();
        return (items, total);
    }

    public async Task<int> GetUltimoSecuencialAsync(long emisorId, string serie)
    {
        var ultimo = await _dbSet.Where(r => r.EmisorId == emisorId && r.Serie == serie)
            .OrderByDescending(r => r.Id).Select(r => r.Secuencial).FirstOrDefaultAsync();
        return ultimo != null && int.TryParse(ultimo, out var num) ? num : 0;
    }
}
