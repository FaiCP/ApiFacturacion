using Application.DTOs.Facturas;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Validators;
using MediatR;

namespace Application.Commands.NotasCredito;

public record CreateNotaCreditoCommand(
    long FacturaId,
    string Motivo,
    DateTime FechaEmision,
    List<DetalleFacturaCreateDto> Detalles) : IRequest<long>;

public class CreateNotaCreditoCommandHandler : IRequestHandler<CreateNotaCreditoCommand, long>
{
    private readonly INotaCreditoRepository _ncRepo;
    private readonly IFacturaRepository _facturaRepo;
    private readonly IEmisorRepository _emisorRepo;

    public CreateNotaCreditoCommandHandler(
        INotaCreditoRepository ncRepo,
        IFacturaRepository facturaRepo,
        IEmisorRepository emisorRepo)
    {
        _ncRepo = ncRepo;
        _facturaRepo = facturaRepo;
        _emisorRepo = emisorRepo;
    }

    public async Task<long> Handle(CreateNotaCreditoCommand request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepo.GetWithDetallesAsync(request.FacturaId)
            ?? throw new NotFoundException($"Factura {request.FacturaId} no encontrada.");

        if (factura.Estado != EstadoSRI.AUTORIZADA)
            throw new DomainException("Solo se puede emitir Nota de Crédito sobre facturas AUTORIZADAS.");

        var emisor = await _emisorRepo.GetByIdAsync(factura.EmisorId)
            ?? throw new NotFoundException("Emisor no encontrado.");

        var serie = $"{emisor.SerieEstablecimiento}-{emisor.SeriePuntoEmision}";
        var ultimoSeq = await _ncRepo.GetUltimoSecuencialAsync(emisor.Id, serie);
        var secuencial = (ultimoSeq + 1).ToString("D9");

        var claveAcceso = ClaveAccesoGenerator.Generar(
            request.FechaEmision, TipoDocumentoSRI.NotaCredito,
            emisor.Ruc, emisor.Ambiente,
            emisor.SerieEstablecimiento, emisor.SeriePuntoEmision,
            int.Parse(secuencial));

        var detalles = request.Detalles.Select(d =>
        {
            var linea = TaxCalculatorService.CalcularLinea(d.Cantidad, d.PrecioUnitario, d.Descuento, d.TarifaIva);
            return new DetalleNotaCredito
            {
                CodigoPrincipal = d.CodigoPrincipal,
                Descripcion = d.Descripcion,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Descuento = d.Descuento,
                TarifaIva = d.TarifaIva,
                SubtotalSinImpuesto = linea.SubtotalSinImpuesto,
                ValorIva = linea.ValorIva,
                PrecioTotalSinImpuesto = linea.PrecioTotalSinImpuesto
            };
        }).ToList();

        var totales = TaxCalculatorService.CalcularTotales(
            detalles.Select(d => new LineaTotales(d.SubtotalSinImpuesto, d.ValorIva, d.PrecioTotalSinImpuesto)));

        var nc = new NotaCredito
        {
            FacturaId = request.FacturaId,
            EmisorId = emisor.Id,
            ClienteId = factura.ClienteId,
            FechaEmision = request.FechaEmision,
            FechaEmisionDocSustento = factura.FechaEmision,
            NumDocModificado = $"{factura.Serie}-{factura.Secuencial}",
            Motivo = request.Motivo,
            Serie = serie,
            Secuencial = secuencial,
            ClaveAcceso = claveAcceso,
            Estado = EstadoSRI.BORRADOR,
            TotalSinImpuestos = totales.TotalSinImpuestos,
            TotalIva = totales.TotalIva,
            ValorModificacion = totales.ImporteTotal,
            Detalles = detalles,
            Borrado = false
        };

        var result = await _ncRepo.AddAsync(nc);
        return result.Id;
    }
}
