using Application.DTOs.Retenciones;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Validators;
using MediatR;

namespace Application.Commands.Retenciones;

public record CreateRetencionCommand(
    long FacturaId, long SujetoRetenidoId,
    DateTime FechaEmision, string PeriodoFiscal,
    List<DetalleRetencionCreateDto> Detalles) : IRequest<long>;

public class CreateRetencionCommandHandler : IRequestHandler<CreateRetencionCommand, long>
{
    private readonly IRetencionRepository _retRepo;
    private readonly IFacturaRepository _facturaRepo;
    private readonly IEmisorRepository _emisorRepo;

    public CreateRetencionCommandHandler(IRetencionRepository retRepo, IFacturaRepository facturaRepo, IEmisorRepository emisorRepo)
    {
        _retRepo = retRepo; _facturaRepo = facturaRepo; _emisorRepo = emisorRepo;
    }

    public async Task<long> Handle(CreateRetencionCommand request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepo.GetWithDetallesAsync(request.FacturaId)
            ?? throw new NotFoundException($"Factura {request.FacturaId} no encontrada.");

        var emisor = await _emisorRepo.GetByIdAsync(factura.EmisorId)
            ?? throw new NotFoundException("Emisor no encontrado.");

        var serie = $"{emisor.SerieEstablecimiento}-{emisor.SeriePuntoEmision}";
        var ultimoSeq = await _retRepo.GetUltimoSecuencialAsync(emisor.Id, serie);
        var secuencial = (ultimoSeq + 1).ToString("D9");

        var claveAcceso = ClaveAccesoGenerator.Generar(
            request.FechaEmision, TipoDocumentoSRI.Retencion,
            emisor.Ruc, emisor.Ambiente,
            emisor.SerieEstablecimiento, emisor.SeriePuntoEmision,
            int.Parse(secuencial));

        var detalles = request.Detalles.Select(d => new DetalleRetencion
        {
            TipoImpuesto = d.TipoImpuesto,
            CodigoRetencion = d.CodigoRetencion,
            BaseImponible = d.BaseImponible,
            PorcentajeRetener = d.PorcentajeRetener,
            ValorRetenido = Math.Round(d.BaseImponible * d.PorcentajeRetener / 100, 2),
            CodDocSustento = "01",
            NumDocSustento = $"{factura.Serie}-{factura.Secuencial}",
            FechaEmisionDocSustento = factura.FechaEmision
        }).ToList();

        var retencion = new Retencion
        {
            FacturaId = request.FacturaId,
            EmisorId = emisor.Id,
            SujetoRetenidoId = request.SujetoRetenidoId,
            FechaEmision = request.FechaEmision,
            PeriodoFiscal = request.PeriodoFiscal,
            Serie = serie, Secuencial = secuencial,
            ClaveAcceso = claveAcceso,
            Estado = EstadoSRI.BORRADOR,
            Detalles = detalles,
            Borrado = false
        };

        var result = await _retRepo.AddAsync(retencion);
        return result.Id;
    }
}
