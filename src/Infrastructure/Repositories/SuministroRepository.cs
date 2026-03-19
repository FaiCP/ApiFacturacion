using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SuministroRepository : GenericRepository<Suministro>, ISuministroRepository
{
    public SuministroRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Suministro>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        IQueryable<Suministro> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(s =>
                (s.IdEquipo != null && s.IdEquipo.ToLower().Contains(searchTerm)) ||
                (s.TipoSuministro != null && s.TipoSuministro.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountSearchAsync(string searchTerm)
    {
        IQueryable<Suministro> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(s =>
                (s.IdEquipo != null && s.IdEquipo.ToLower().Contains(searchTerm)) ||
                (s.TipoSuministro != null && s.TipoSuministro.ToLower().Contains(searchTerm)));
        }

        return await query.CountAsync();
    }
}
