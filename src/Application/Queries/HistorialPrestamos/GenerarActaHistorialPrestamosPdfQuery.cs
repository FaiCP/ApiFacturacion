using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.HistorialPrestamos;

public record GenerarActaHistorialPrestamosPdfQuery(List<long> Ids) : IRequest<byte[]>;

public class GenerarActaHistorialPrestamosPdfQueryHandler : IRequestHandler<GenerarActaHistorialPrestamosPdfQuery, byte[]>
{
    private readonly IGestionActivoRepository _repo;
    private readonly IPdfService _pdfService;

    public GenerarActaHistorialPrestamosPdfQueryHandler(IGestionActivoRepository repo, IPdfService pdfService)
    {
        _repo = repo;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GenerarActaHistorialPrestamosPdfQuery request, CancellationToken cancellationToken)
    {
        var items = await _repo.GetByIdsAsync(request.Ids, borrado: true);
        if (!items.Any()) throw new NotFoundException("HistorialPrestamo", string.Join(",", request.Ids));

        var dtos = items.Select(g => new HistorialActaItemDto(
            g.IdEquipo,
            g.Hardware?.NombreDispositivo ?? "",
            g.Hardware?.CodigoCne ?? "",
            g.Hardware?.Modelo ?? "",
            g.Hardware?.Marca ?? "",
            g.Hardware?.Valor,
            g.Hardware?.Estado ?? "",
            g.Hardware?.Observacion ?? "",
            g.Custodio?.Nombre ?? "",
            g.Custodio?.Cargo ?? "",
            g.Custodio?.Departamento?.Nombre ?? ""
        )).ToList();

        return _pdfService.GenerarActaHistorialPrestamos(dtos.First(), dtos);
    }
}
