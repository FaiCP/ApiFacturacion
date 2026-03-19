namespace Domain.Interfaces;

/// <summary>
/// Interfaz de repositorio para Hardware
/// </summary>
public interface IHardwareRepository : IGenericRepository<Entities.Hardware>
{
    Task<Entities.Hardware?> GetByIdEquipoAsync(string idEquipo);
    Task<IReadOnlyList<Entities.Hardware>> GetByEstadoAsync(string estado);
    Task<IReadOnlyList<Entities.Hardware>> SearchAsync(string searchTerm, int page, int pageSize);
    Task<int> CountSearchAsync(string searchTerm);
    Task<IReadOnlyList<Entities.Hardware>> GetAllWithCaracteristicasAsync();
}
