using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PersonalRepository : GenericRepository<Persona>, IPersonalRepository
{
    public PersonalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Persona>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        IQueryable<Persona> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(searchTerm)) ||
                (p.Cedula != null && p.Cedula.ToLower().Contains(searchTerm)) ||
                (p.Cargo != null && p.Cargo.ToLower().Contains(searchTerm)) ||
                (p.Email != null && p.Email.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountSearchAsync(string searchTerm)
    {
        IQueryable<Persona> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(searchTerm)) ||
                (p.Cedula != null && p.Cedula.ToLower().Contains(searchTerm)) ||
                (p.Cargo != null && p.Cargo.ToLower().Contains(searchTerm)) ||
                (p.Email != null && p.Email.ToLower().Contains(searchTerm)));
        }

        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<Persona>> GetByIdsAsync(List<long> ids)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Persona>> GetAllActiveAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();
    }
}
