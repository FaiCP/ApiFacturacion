using Application.DTOs.Documentos;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Personal;

public record GenerarReportePersonalPdfQuery : IRequest<byte[]>;

public class GenerarReportePersonalPdfQueryHandler : IRequestHandler<GenerarReportePersonalPdfQuery, byte[]>
{
    private readonly IGestionActivoRepository _gestionRepo;
    private readonly IPersonalRepository _personalRepo;
    private readonly IPdfService _pdfService;

    public GenerarReportePersonalPdfQueryHandler(IGestionActivoRepository gestionRepo, IPersonalRepository personalRepo, IPdfService pdfService)
    {
        _gestionRepo = gestionRepo;
        _personalRepo = personalRepo;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GenerarReportePersonalPdfQuery request, CancellationToken cancellationToken)
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

            if (g.FechaDevolucion >= fechaLimite && g.FechaDevolucion?.Year == añoActual)
            {
                reporte.Add(new ReportePersonalItemDto(
                    g.FechaDevolucion,
                    custodio,
                    "NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL",
                    "X", null,
                    $"{g.Hardware?.NombreDispositivo}, Marca:{g.Hardware?.Marca}, Modelo:{g.Hardware?.Modelo}, Serie:{g.Hardware?.CodigoCne}"
                ));
            }
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

        return await _pdfService.GenerarReportePersonalAsync(reporte);
    }
}
