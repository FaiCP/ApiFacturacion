using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Hardware;

public record GenerarActaHardwareExcelQuery : IRequest<byte[]>;

public class GenerarActaHardwareExcelQueryHandler : IRequestHandler<GenerarActaHardwareExcelQuery, byte[]>
{
    private readonly IHardwareRepository _hardwareRepo;
    private readonly IExcelService _excelService;

    public GenerarActaHardwareExcelQueryHandler(IHardwareRepository hardwareRepo, IExcelService excelService)
    {
        _hardwareRepo = hardwareRepo;
        _excelService = excelService;
    }

    public async Task<byte[]> Handle(GenerarActaHardwareExcelQuery request, CancellationToken cancellationToken)
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
        return _excelService.GenerarActaHardware(dtos);
    }
}
