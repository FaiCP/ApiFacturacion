using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmisorRepository : GenericRepository<Emisor>, IEmisorRepository
{
    public EmisorRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Emisor?> GetActivoAsync() =>
        await _dbSet.AsNoTracking()
            .Where(e => e.Borrado != true)
            .FirstOrDefaultAsync();
}
