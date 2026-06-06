using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface IFacturaRepository : IGenericRepository<Factura>
{
    Task<Factura?> GetByClaveAccesoAsync(string claveAcceso);
    Task<Factura?> GetWithDetallesAsync(long id);
    Task<(IReadOnlyList<Factura> Items, int Total)> GetPagedAsync(int pagina, int cantidad, EstadoSRI? estado, DateTime? desde, DateTime? hasta, long? clienteId);
    Task<int> GetUltimoSecuencialAsync(long emisorId, string serie);
}
