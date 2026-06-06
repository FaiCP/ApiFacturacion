using Domain.Entities;

namespace Domain.Interfaces;

public interface IConfiguracionSRIRepository : IGenericRepository<ConfiguracionSRI>
{
    Task<ConfiguracionSRI?> GetActivaAsync();
}
