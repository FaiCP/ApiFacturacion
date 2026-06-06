using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Facturas;

public record AnularFacturaCommand(long Id) : IRequest<bool>;

public class AnularFacturaCommandHandler : IRequestHandler<AnularFacturaCommand, bool>
{
    private readonly IFacturaRepository _facturaRepository;

    public AnularFacturaCommandHandler(IFacturaRepository facturaRepository)
    {
        _facturaRepository = facturaRepository;
    }

    public async Task<bool> Handle(AnularFacturaCommand request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Factura con Id {request.Id} no encontrada.");

        if (factura.Estado == EstadoSRI.ANULADA)
            throw new DomainException("La factura ya está anulada.");

        if (factura.Estado == EstadoSRI.AUTORIZADA)
            throw new DomainException("No se puede anular una factura autorizada directamente. Emita una Nota de Crédito.");

        factura.Estado = EstadoSRI.ANULADA;
        factura.UpdatedAt = DateTime.UtcNow;

        await _facturaRepository.UpdateAsync(factura);
        return true;
    }
}
