using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class KitRepository : GenericRepository<Kit>, IKitRepository
{
    public KitRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Kit>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(k =>
                (k.Insumo != null && k.Insumo.ToLower().Contains(searchTerm)) ||
                (k.Marca != null && k.Marca.ToLower().Contains(searchTerm)) ||
                (k.Modelo != null && k.Modelo.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(k => k.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountSearchAsync(string searchTerm)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(k =>
                (k.Insumo != null && k.Insumo.ToLower().Contains(searchTerm)) ||
                (k.Marca != null && k.Marca.ToLower().Contains(searchTerm)) ||
                (k.Modelo != null && k.Modelo.ToLower().Contains(searchTerm)));
        }

        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<Kit>> GetAllActiveAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(k => k.Id)
            .ToListAsync();
    }
}
