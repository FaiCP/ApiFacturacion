using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Personal;

public record GenerarReportePersonalExcelQuery : IRequest<byte[]>;

public class GenerarReportePersonalExcelQueryHandler : IRequestHandler<GenerarReportePersonalExcelQuery, byte[]>
{
    private readonly IGestionActivoRepository _gestionRepo;
    private readonly IPersonalRepository _personalRepo;
    private readonly IExcelService _excelService;

    public GenerarReportePersonalExcelQueryHandler(IGestionActivoRepository gestionRepo, IPersonalRepository personalRepo, IExcelService excelService)
    {
        _gestionRepo = gestionRepo;
        _personalRepo = personalRepo;
        _excelService = excelService;
    }

    public async Task<byte[]> Handle(GenerarReportePersonalExcelQuery request, CancellationToken cancellationToken)
    {
        var fechaLimite = DateTime.Now.AddMonths(-6);
        var añoActual = DateTime.Now.Year;

        var equipos = await _gestionRepo.GetAllActiveWithDetailsAsync();
        var personal = await _personalRepo.GetAllActiveAsync();

        var reporte = new List<ReportePersonalItemDto>();

        foreach (var g in equipos.Where(e => e.FechaAsignacion >= fechaLimite && e.FechaAsignacion?.Year == añoActual))
        {
            var custodio = g.Custodio?.Nombre ?? "";
            reporte.Add(new ReportePersonalItemDto(
                g.FechaAsignacion,
                "NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL",
                custodio,
                "X", null,
                $"{g.Hardware?.NombreDispositivo}, Marca:{g.Hardware?.Marca}, Modelo:{g.Hardware?.Modelo}, Serie:{g.Hardware?.CodigoCne}"
            ));
        }

        foreach (var p in personal.Where(p => p.Fecha >= fechaLimite && p.Fecha?.Year == añoActual))
        {
            reporte.Add(new ReportePersonalItemDto(
                p.Fecha,
                "NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL",
                p.Nombre,
                null, "X",
                "Credenciales Zimbra y Quipux"
            ));
        }

        return _excelService.GenerarReportePersonal(reporte);
    }
}
