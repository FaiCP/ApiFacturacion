using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.GestionActivos;

public record GenerarActaGestionActivosPdfQuery(List<long> Ids) : IRequest<byte[]>;

public class GenerarActaGestionActivosPdfQueryHandler : IRequestHandler<GenerarActaGestionActivosPdfQuery, byte[]>
{
    private readonly IGestionActivoRepository _repo;
    private readonly IPdfService _pdfService;

    public GenerarActaGestionActivosPdfQueryHandler(IGestionActivoRepository repo, IPdfService pdfService)
    {
        _repo = repo;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GenerarActaGestionActivosPdfQuery request, CancellationToken cancellationToken)
    {
        var items = await _repo.GetByIdsAsync(request.Ids, borrado: false);
        if (!items.Any()) throw new NotFoundException("GestionActivo", string.Join(",", request.Ids));

        var dtos = items.Select(g => new GestionActivoActaItemDto(
            g.IdEquipo,
            g.FechaAsignacion ?? DateTime.Now,
            g.Hardware?.NombreDispositivo ?? "",
            g.Hardware?.Marca ?? "",
            g.Hardware?.Modelo ?? "",
            g.Hardware?.CodigoCne ?? "",
            g.Hardware?.Estado ?? "",
            g.Custodio?.Nombre ?? "",
            g.Custodio?.Cargo ?? "",
            g.Custodio?.Departamento?.Nombre ?? ""
        )).ToList();

        return _pdfService.GenerarActaGestionActivos(dtos.First(), dtos);
    }
}
