using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface INotaDebitoRepository : IGenericRepository<NotaDebito>
{
    Task<NotaDebito?> GetWithMotivosAsync(long id);
    Task<(IReadOnlyList<NotaDebito> Items, int Total)> GetPagedAsync(int pagina, int cantidad, EstadoSRI? estado, long? facturaId);
    Task<int> GetUltimoSecuencialAsync(long emisorId, string serie);
}
