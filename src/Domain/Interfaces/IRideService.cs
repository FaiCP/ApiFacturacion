using Domain.Entities;

namespace Domain.Interfaces;

public interface IRideService
{
    Task<byte[]> GenerarRideAsync(Factura factura);
}
