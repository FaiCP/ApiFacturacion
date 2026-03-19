using Application.DTOs.Reportes;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reportes;

public record GetEquiposPrestadosPorTipoQuery : IRequest<List<ReportePrestadosDto>>;

public class GetEquiposPrestadosPorTipoQueryHandler : IRequestHandler<GetEquiposPrestadosPorTipoQuery, List<ReportePrestadosDto>>
{
    private readonly IGestionActivoRepository _repo;
    public GetEquiposPrestadosPorTipoQueryHandler(IGestionActivoRepository repo) => _repo = repo;

    public async Task<List<ReportePrestadosDto>> Handle(GetEquiposPrestadosPorTipoQuery request, CancellationToken cancellationToken)
    {
        var all = await _repo.GetAllActiveWithDetailsAsync();
        var cultura = new System.Globalization.CultureInfo("es-ES");

        return all
            .Where(g => g.FechaAsignacion.HasValue && g.FechaDevolucion == null && g.Hardware != null)
            .GroupBy(g => new {
                g.Hardware!.NombreDispositivo,
                Mes = g.FechaAsignacion!.Value.Month,
                Año = g.FechaAsignacion!.Value.Year
            })
            .Select(g => new ReportePrestadosDto(
                g.Key.NombreDispositivo,
                g.Count(),
                new DateTime(g.Key.Año, g.Key.Mes, 1).ToString("MMMM", cultura),
                g.Key.Año))
            .ToList();
    }
}
