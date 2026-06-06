using Application.DTOs.Facturas;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Validators;
using MediatR;

namespace Application.Commands.Facturas;

public record CreateFacturaCommand(long ClienteId, DateTime FechaEmision, List<DetalleFacturaCreateDto> Detalles) : IRequest<long>;

public class CreateFacturaCommandHandler : IRequestHandler<CreateFacturaCommand, long>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IEmisorRepository _emisorRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly FacturaNumeracionService _numeracion;

    public CreateFacturaCommandHandler(
        IFacturaRepository facturaRepository,
        IEmisorRepository emisorRepository,
        IClienteRepository clienteRepository,
        FacturaNumeracionService numeracion)
    {
        _facturaRepository = facturaRepository;
        _emisorRepository = emisorRepository;
        _clienteRepository = clienteRepository;
        _numeracion = numeracion;
    }

    public async Task<long> Handle(CreateFacturaCommand request, CancellationToken cancellationToken)
    {
        var emisor = await _emisorRepository.GetActivoAsync()
            ?? throw new NotFoundException("No existe emisor configurado. Configure el emisor antes de facturar.");

        _ = await _clienteRepository.GetByIdAsync(request.ClienteId)
            ?? throw new NotFoundException($"Cliente con Id {request.ClienteId} no encontrado.");

        var (serie, secuencial) = await _numeracion.GenerarSiguienteAsync(
            emisor.Id, emisor.SerieEstablecimiento, emisor.SeriePuntoEmision);

        var claveAcceso = ClaveAccesoGenerator.Generar(
            request.FechaEmision,
            TipoDocumentoSRI.Factura,
            emisor.Ruc,
            emisor.Ambiente,
            emisor.SerieEstablecimiento,
            emisor.SeriePuntoEmision,
            int.Parse(secuencial));

        var detalles = request.Detalles.Select(d =>
        {
            var linea = TaxCalculatorService.CalcularLinea(d.Cantidad, d.PrecioUnitario, d.Descuento, d.TarifaIva);
            return new DetalleFactura
            {
                ProductoId = d.ProductoId,
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
            detalles.Select(d => new LineaTotales(d.SubtotalSinImpuesto, d.ValorIva, d.PrecioTotalSinImpuesto)),
            detalles.Sum(d => d.Descuento));

        var factura = new Factura
        {
            EmisorId = emisor.Id,
            ClienteId = request.ClienteId,
            FechaEmision = request.FechaEmision,
            Serie = serie,
            Secuencial = secuencial,
            ClaveAcceso = claveAcceso,
            Estado = EstadoSRI.BORRADOR,
            TotalSinImpuestos = totales.TotalSinImpuestos,
            TotalDescuento = totales.TotalDescuento,
            TotalIva = totales.TotalIva,
            ImporteTotal = totales.ImporteTotal,
            Detalles = detalles,
            Borrado = false
        };

        var result = await _facturaRepository.AddAsync(factura);
        return result.Id;
    }
}
