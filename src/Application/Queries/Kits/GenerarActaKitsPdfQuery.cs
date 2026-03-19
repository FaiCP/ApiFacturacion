using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Kits;

public record GenerarActaKitsPdfQuery : IRequest<byte[]>;

public class GenerarActaKitsPdfQueryHandler : IRequestHandler<GenerarActaKitsPdfQuery, byte[]>
{
    private readonly IKitRepository _kitRepo;
    private readonly IPdfService _pdfService;

    public GenerarActaKitsPdfQueryHandler(IKitRepository kitRepo, IPdfService pdfService)
    {
        _kitRepo = kitRepo;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GenerarActaKitsPdfQuery request, CancellationToken cancellationToken)
    {
        var kits = await _kitRepo.GetAllActiveAsync();
        var dtos = kits.Select(k => new KitActaItemDto(k.Insumo, k.Marca, k.Modelo, k.Serie, k.Cantidad, k.Estado, k.Observacion)).ToList();
        return _pdfService.GenerarActaKits(dtos);
    }
}
