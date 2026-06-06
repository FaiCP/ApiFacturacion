using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class NotaCreditoRepository : GenericRepository<NotaCredito>, INotaCreditoRepository
{
    public NotaCreditoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<NotaCredito?> GetWithDetallesAsync(long id) =>
        await _dbSet.AsNoTracking()
            .Include(n => n.Cliente)
            .Include(n => n.Emisor)
            .Include(n => n.Detalles)
            .FirstOrDefaultAsync(n => n.Id == id);

    public async Task<(IReadOnlyList<NotaCredito> Items, int Total)> GetPagedAsync(int pagina, int cantidad, EstadoSRI? estado, long? facturaId)
    {
        var page = pagina <= 0 ? 1 : pagina;
        IQueryable<NotaCredito> query = _dbSet.AsNoTracking()
            .Include(n => n.Cliente)
            .Where(n => n.Borrado != true);

        if (estado.HasValue) query = query.Where(n => n.Estado == estado.Value);
        if (facturaId.HasValue) query = query.Where(n => n.FacturaId == facturaId.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(n => n.FechaEmision).Skip((page - 1) * cantidad).Take(cantidad).ToListAsync();
        return (items, total);
    }

    public async Task<int> GetUltimoSecuencialAsync(long emisorId, string serie)
    {
        var ultimo = await _dbSet.Where(n => n.EmisorId == emisorId && n.Serie == serie)
            .OrderByDescending(n => n.Id).Select(n => n.Secuencial).FirstOrDefaultAsync();
        return ultimo != null && int.TryParse(ultimo, out var num) ? num : 0;
    }
}
