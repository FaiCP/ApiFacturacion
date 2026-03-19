using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implementación de repositorio para Custodios
/// </summary>
public class CustodioRepository : GenericRepository<Custodio>, ICustodioRepository
{
    public CustodioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Custodio>> GetByDepartamentoAsync(long departamentoId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.IdDepartamento == departamentoId)
            .ToListAsync();
    }

    public async Task<Custodio?> GetByCedulaAsync(string cedula)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Cedula == cedula);
    }

    public async Task<IReadOnlyList<Custodio>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        IQueryable<Custodio> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(c =>
                (c.Nombre != null && c.Nombre.ToLower().Contains(searchTerm)) ||
                (c.Cedula != null && c.Cedula.ToLower().Contains(searchTerm)) ||
                (c.Cargo != null && c.Cargo.ToLower().Contains(searchTerm)));
        }

        return await query
            .Include(c => c.Departamento)
            .OrderByDescending(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountSearchAsync(string searchTerm)
    {
        IQueryable<Custodio> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(c =>
                (c.Nombre != null && c.Nombre.ToLower().Contains(searchTerm)) ||
                (c.Cedula != null && c.Cedula.ToLower().Contains(searchTerm)) ||
                (c.Cargo != null && c.Cargo.ToLower().Contains(searchTerm)));
        }

        return await query.CountAsync();
    }
}
