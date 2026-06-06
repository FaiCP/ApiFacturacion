using Application.DTOs.Reportes;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reportes;

public record GetFacturasPorMesQuery(int? Anio = null) : IRequest<List<FacturasPorMesDto>>;

public class GetFacturasPorMesQueryHandler : IRequestHandler<GetFacturasPorMesQuery, List<FacturasPorMesDto>>
{
    private readonly IReporteFacturacionRepository _repo;
    public GetFacturasPorMesQueryHandler(IReporteFacturacionRepository repo) => _repo = repo;

    public async Task<List<FacturasPorMesDto>> Handle(GetFacturasPorMesQuery request, CancellationToken ct)
    {
        var anio = request.Anio ?? DateTime.UtcNow.Year;
        var data = await _repo.GetFacturasPorMesAsync(anio);
        return data.Select(d => new FacturasPorMesDto(
            d.Anio, d.Mes,
            new DateTime(d.Anio, d.Mes, 1).ToString("MMMM", new System.Globalization.CultureInfo("es-EC")),
            d.Cantidad, d.TotalMonto, d.TotalIva, d.TotalSinImpuestos))
            .ToList();
    }
}
