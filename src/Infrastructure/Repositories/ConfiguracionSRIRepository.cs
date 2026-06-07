using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ConfiguracionSRIRepository : GenericRepository<ConfiguracionSRI>, IConfiguracionSRIRepository
{
    public ConfiguracionSRIRepository(ApplicationDbContext context) : base(context) { }

    public async Task<ConfiguracionSRI?> GetActivaPorEmisorAsync(long emisorId) =>
        await _dbSet.AsNoTracking()
            .Where(c => c.EmisorId == emisorId && c.Activo && c.Borrado != true)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();
}
