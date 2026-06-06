using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Cliente?> GetByIdentificacionAsync(string numeroIdentificacion) =>
        await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(c => c.NumeroIdentificacion == numeroIdentificacion && c.Borrado != true);

    public async Task<(IReadOnlyList<Cliente> Items, int Total)> GetPagedAsync(int pagina, int cantidad, string? busqueda)
    {
        var page = pagina <= 0 ? 1 : pagina;
        IQueryable<Cliente> query = _dbSet.AsNoTracking().Where(c => c.Borrado != true);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var term = busqueda.ToLower();
            query = query.Where(c =>
                c.RazonSocial.ToLower().Contains(term) ||
                c.NumeroIdentificacion.Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.Id)
            .Skip((page - 1) * cantidad)
            .Take(cantidad)
            .ToListAsync();

        return (items, total);
    }
}
