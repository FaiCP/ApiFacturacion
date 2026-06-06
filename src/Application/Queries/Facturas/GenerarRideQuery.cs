using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Facturas;

public record GenerarRideQuery(long FacturaId) : IRequest<byte[]>;

public class GenerarRideQueryHandler : IRequestHandler<GenerarRideQuery, byte[]>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IRideService _rideService;

    public GenerarRideQueryHandler(IFacturaRepository facturaRepository, IRideService rideService)
    {
        _facturaRepository = facturaRepository;
        _rideService = rideService;
    }

    public async Task<byte[]> Handle(GenerarRideQuery request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.GetWithDetallesAsync(request.FacturaId)
            ?? throw new NotFoundException($"Factura con Id {request.FacturaId} no encontrada.");
        return await _rideService.GenerarRideAsync(factura);
    }
}
