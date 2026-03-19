using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Hardware;

public record GenerarActaHardwarePdfQuery : IRequest<byte[]>;

public class GenerarActaHardwarePdfQueryHandler : IRequestHandler<GenerarActaHardwarePdfQuery, byte[]>
{
    private readonly IHardwareRepository _hardwareRepo;
    private readonly IPdfService _pdfService;

    public GenerarActaHardwarePdfQueryHandler(IHardwareRepository hardwareRepo, IPdfService pdfService)
    {
        _hardwareRepo = hardwareRepo;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GenerarActaHardwarePdfQuery request, CancellationToken cancellationToken)
    {
        var items = await _hardwareRepo.GetAllWithCaracteristicasAsync();
        var dtos = items.Select(h => new HardwareActaItemDto(
            h.IdEquipo,
            h.GestionActivos.FirstOrDefault(g => g.Borrado != true)?.Custodio?.Nombre ?? "",
            h.NombreDispositivo,
            h.Marca,
            h.Modelo,
            h.CodigoCne,
            h.Caracteristicas.FirstOrDefault()?.Ram,
            h.Caracteristicas.FirstOrDefault()?.Rom,
            h.Caracteristicas.FirstOrDefault()?.Procesador,
            h.Valor,
            h.Estado
        )).ToList();
        return _pdfService.GenerarActaHardware(dtos);
    }
}
