using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface INotaCreditoRepository : IGenericRepository<NotaCredito>
{
    Task<NotaCredito?> GetWithDetallesAsync(long id);
    Task<(IReadOnlyList<NotaCredito> Items, int Total)> GetPagedAsync(int pagina, int cantidad, EstadoSRI? estado, long? facturaId);
    Task<int> GetUltimoSecuencialAsync(long emisorId, string serie);
}
