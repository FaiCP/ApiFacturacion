using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface IRetencionRepository : IGenericRepository<Retencion>
{
    Task<Retencion?> GetWithDetallesAsync(long id);
    Task<(IReadOnlyList<Retencion> Items, int Total)> GetPagedAsync(int pagina, int cantidad, EstadoSRI? estado, long? facturaId);
    Task<int> GetUltimoSecuencialAsync(long emisorId, string serie);
}
