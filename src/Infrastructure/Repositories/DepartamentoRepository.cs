using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DepartamentoRepository : GenericRepository<Departamento>, IDepartamentoRepository
{
    public DepartamentoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Departamento>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(d => d.Nombre != null && d.Nombre.ToLower().Contains(searchTerm));
        }

        return await query
            .OrderByDescending(d => d.Id)
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
            query = query.Where(d => d.Nombre != null && d.Nombre.ToLower().Contains(searchTerm));
        }

        return await query.CountAsync();
    }
}
