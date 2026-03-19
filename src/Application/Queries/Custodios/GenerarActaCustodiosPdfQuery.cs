using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Custodios;

public record GenerarActaCustodiosPdfQuery : IRequest<byte[]>;

public class GenerarActaCustodiosPdfQueryHandler : IRequestHandler<GenerarActaCustodiosPdfQuery, byte[]>
{
    private readonly IGestionActivoRepository _repo;
    private readonly IPdfService _pdfService;

    public GenerarActaCustodiosPdfQueryHandler(IGestionActivoRepository repo, IPdfService pdfService)
    {
        _repo = repo;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GenerarActaCustodiosPdfQuery request, CancellationToken cancellationToken)
    {
        var items = await _repo.GetAllActiveWithDetailsAsync();
        var dtos = items.Select(g => new CustodioActaItemDto(
            g.IdEquipo,
            g.FechaAsignacion ?? DateTime.Now,
            g.Custodio?.Nombre ?? "",
            g.Hardware?.Marca ?? "",
            g.Hardware?.Modelo ?? "",
            g.Hardware?.CodigoCne ?? "",
            g.Hardware?.Valor,
            g.Hardware?.Estado ?? "",
            g.Hardware?.NombreDispositivo ?? ""
        )).ToList();
        return _pdfService.GenerarActaCustodios(dtos);
    }
}
