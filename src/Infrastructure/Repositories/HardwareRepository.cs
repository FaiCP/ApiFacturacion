using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implementación de repositorio para Hardware
/// </summary>
public class HardwareRepository : GenericRepository<Hardware>, IHardwareRepository
{
    public HardwareRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Hardware?> GetByIdEquipoAsync(string idEquipo)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.IdEquipo == idEquipo);
    }

    public async Task<IReadOnlyList<Hardware>> GetByEstadoAsync(string estado)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(h => h.Estado == estado)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Hardware>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        IQueryable<Hardware> query = _dbSet
            .AsNoTracking()
            .Include(h => h.GestionActivos.Where(g => g.FechaDevolucion == null))
                .ThenInclude(g => g.Custodio);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(h =>
                (h.IdEquipo != null && h.IdEquipo.ToLower().Contains(searchTerm)) ||
                (h.Marca != null && h.Marca.ToLower().Contains(searchTerm)) ||
                (h.Modelo != null && h.Modelo.ToLower().Contains(searchTerm)) ||
                (h.NombreDispositivo != null && h.NombreDispositivo.ToLower().Contains(searchTerm)) ||
                (h.CodigoCne != null && h.CodigoCne.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderByDescending(h => h.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountSearchAsync(string searchTerm)
    {
        IQueryable<Hardware> query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(h =>
                (h.IdEquipo != null && h.IdEquipo.ToLower().Contains(searchTerm)) ||
                (h.Marca != null && h.Marca.ToLower().Contains(searchTerm)) ||
                (h.Modelo != null && h.Modelo.ToLower().Contains(searchTerm)) ||
                (h.NombreDispositivo != null && h.NombreDispositivo.ToLower().Contains(searchTerm)) ||
                (h.CodigoCne != null && h.CodigoCne.ToLower().Contains(searchTerm)));
        }

        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<Hardware>> GetAllWithCaracteristicasAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(h => h.Caracteristicas)
            .Include(h => h.GestionActivos)
                .ThenInclude(g => g.Custodio)
            .OrderBy(h => h.Id)
            .ToListAsync();
    }
}
