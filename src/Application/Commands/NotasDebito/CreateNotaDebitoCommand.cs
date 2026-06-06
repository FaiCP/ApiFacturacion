using Application.DTOs.NotasDebito;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Validators;
using MediatR;

namespace Application.Commands.NotasDebito;

public record CreateNotaDebitoCommand(long FacturaId, DateTime FechaEmision, List<MotivoDto> Motivos) : IRequest<long>;

public class CreateNotaDebitoCommandHandler : IRequestHandler<CreateNotaDebitoCommand, long>
{
    private readonly INotaDebitoRepository _ndRepo;
    private readonly IFacturaRepository _facturaRepo;
    private readonly IEmisorRepository _emisorRepo;

    public CreateNotaDebitoCommandHandler(INotaDebitoRepository ndRepo, IFacturaRepository facturaRepo, IEmisorRepository emisorRepo)
    {
        _ndRepo = ndRepo; _facturaRepo = facturaRepo; _emisorRepo = emisorRepo;
    }

    public async Task<long> Handle(CreateNotaDebitoCommand request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepo.GetWithDetallesAsync(request.FacturaId)
            ?? throw new NotFoundException($"Factura {request.FacturaId} no encontrada.");

        if (factura.Estado != EstadoSRI.AUTORIZADA)
            throw new DomainException("Solo se puede emitir Nota de Débito sobre facturas AUTORIZADAS.");

        var emisor = await _emisorRepo.GetByIdAsync(factura.EmisorId)
            ?? throw new NotFoundException("Emisor no encontrado.");

        var serie = $"{emisor.SerieEstablecimiento}-{emisor.SeriePuntoEmision}";
        var ultimoSeq = await _ndRepo.GetUltimoSecuencialAsync(emisor.Id, serie);
        var secuencial = (ultimoSeq + 1).ToString("D9");

        var claveAcceso = ClaveAccesoGenerator.Generar(
            request.FechaEmision, TipoDocumentoSRI.NotaDebito,
            emisor.Ruc, emisor.Ambiente,
            emisor.SerieEstablecimiento, emisor.SeriePuntoEmision,
            int.Parse(secuencial));

        var totalSinImp = request.Motivos.Sum(m => m.Valor);
        var totalIva = Math.Round(totalSinImp * 0.15m, 2);

        var nd = new NotaDebito
        {
            FacturaId = request.FacturaId,
            EmisorId = emisor.Id,
            ClienteId = factura.ClienteId,
            FechaEmision = request.FechaEmision,
            FechaEmisionDocSustento = factura.FechaEmision,
            NumDocModificado = $"{factura.Serie}-{factura.Secuencial}",
            Serie = serie, Secuencial = secuencial,
            ClaveAcceso = claveAcceso,
            Estado = EstadoSRI.BORRADOR,
            TotalSinImpuestos = totalSinImp,
            TotalIva = totalIva,
            ValorTotal = totalSinImp + totalIva,
            Motivos = request.Motivos.Select(m => new MotivoNotaDebito { Razon = m.Razon, Valor = m.Valor }).ToList(),
            Borrado = false
        };

        var result = await _ndRepo.AddAsync(nd);
        return result.Id;
    }
}
