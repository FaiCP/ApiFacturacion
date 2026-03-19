using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GestionActivoRepository : GenericRepository<GestionActivo>, IGestionActivoRepository
{
    public GestionActivoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<GestionActivo>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(g => g.Custodio)
            .Include(g => g.Hardware)
            .Where(g => g.FechaDevolucion == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(g =>
                (g.IdEquipo != null && g.IdEquipo.ToLower().Contains(searchTerm)) ||
                (g.Custodio != null && g.Custodio.Nombre != null && g.Custodio.Nombre.ToLower().Contains(searchTerm)) ||
                (g.Hardware != null && g.Hardware.Marca != null && g.Hardware.Marca.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(g => g.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountSearchAsync(string searchTerm)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(g => g.Custodio)
            .Include(g => g.Hardware)
            .Where(g => g.FechaDevolucion == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(g =>
                (g.IdEquipo != null && g.IdEquipo.ToLower().Contains(searchTerm)) ||
                (g.Custodio != null && g.Custodio.Nombre != null && g.Custodio.Nombre.ToLower().Contains(searchTerm)) ||
                (g.Hardware != null && g.Hardware.Marca != null && g.Hardware.Marca.ToLower().Contains(searchTerm)));
        }

        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<GestionActivo>> GetHistorialAsync(string searchTerm, int page, int pageSize, int idCustodio)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(g => g.Custodio)
            .Include(g => g.Hardware)
            .Where(g => g.FechaDevolucion != null);

        if (idCustodio > 0)
        {
            query = query.Where(g => g.IdCustodio == idCustodio);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(g =>
                (g.IdEquipo != null && g.IdEquipo.ToLower().Contains(searchTerm)) ||
                (g.Custodio != null && g.Custodio.Nombre != null && g.Custodio.Nombre.ToLower().Contains(searchTerm)) ||
                (g.Hardware != null && g.Hardware.Marca != null && g.Hardware.Marca.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(g => g.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountHistorialAsync(string searchTerm, int idCustodio)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(g => g.Custodio)
            .Include(g => g.Hardware)
            .Where(g => g.FechaDevolucion != null);

        if (idCustodio > 0)
        {
            query = query.Where(g => g.IdCustodio == idCustodio);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(g =>
                (g.IdEquipo != null && g.IdEquipo.ToLower().Contains(searchTerm)) ||
                (g.Custodio != null && g.Custodio.Nombre != null && g.Custodio.Nombre.ToLower().Contains(searchTerm)) ||
                (g.Hardware != null && g.Hardware.Marca != null && g.Hardware.Marca.ToLower().Contains(searchTerm)));
        }

        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<GestionActivo>> GetAllActiveWithDetailsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Hardware)
            .Include(g => g.Custodio)
                .ThenInclude(c => c.Departamento)
            .OrderBy(g => g.Id)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<GestionActivo>> GetByIdsAsync(List<long> ids, bool borrado = false)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Hardware)
            .Include(g => g.Custodio)
                .ThenInclude(c => c.Departamento)
            .Where(g => ids.Contains(g.Id))
            .OrderBy(g => g.Id)
            .ToListAsync();
    }
}
