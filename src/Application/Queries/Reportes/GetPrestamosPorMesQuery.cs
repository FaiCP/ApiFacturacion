using Application.DTOs.Reportes;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reportes;

public record GetPrestamosPorMesQuery : IRequest<List<ReportePrestamosDto>>;

public class GetPrestamosPorMesQueryHandler : IRequestHandler<GetPrestamosPorMesQuery, List<ReportePrestamosDto>>
{
    private readonly IGestionActivoRepository _repo;
    public GetPrestamosPorMesQueryHandler(IGestionActivoRepository repo) => _repo = repo;

    public async Task<List<ReportePrestamosDto>> Handle(GetPrestamosPorMesQuery request, CancellationToken cancellationToken)
    {
        var all = await _repo.GetAllAsync();
        var cultura = new System.Globalization.CultureInfo("es-ES");

        return all
            .Where(a => a.FechaAsignacion.HasValue)
            .GroupBy(a => a.FechaAsignacion!.Value.ToString("MMMM", cultura))
            .Select(g => new ReportePrestamosDto(
                g.Key,
                g.Count(),
                g.Count(a => a.FechaDevolucion.HasValue)))
            .OrderBy(r => r.Mes)
            .ToList();
    }
}
