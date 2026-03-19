using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Personal;

public record GenerarActaPersonalPdfQuery(List<long> Ids) : IRequest<byte[]>;

public class GenerarActaPersonalPdfQueryHandler : IRequestHandler<GenerarActaPersonalPdfQuery, byte[]>
{
    private readonly IPersonalRepository _personalRepo;
    private readonly IPdfService _pdfService;

    public GenerarActaPersonalPdfQueryHandler(IPersonalRepository personalRepo, IPdfService pdfService)
    {
        _personalRepo = personalRepo;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GenerarActaPersonalPdfQuery request, CancellationToken cancellationToken)
    {
        var personas = await _personalRepo.GetByIdsAsync(request.Ids);
        var first = personas.FirstOrDefault();
        if (first == null) throw new Domain.Exceptions.NotFoundException("Personal", string.Join(",", request.Ids));

        var dto = new PersonalActaItemDto(first.Nombre, first.Cedula, first.Cargo, first.Fecha, first.Email, first.TempPass);
        return _pdfService.GenerarActaPersonal(dto);
    }
}
