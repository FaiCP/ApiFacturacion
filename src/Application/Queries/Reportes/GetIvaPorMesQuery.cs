using Application.DTOs.Reportes;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reportes;

public record GetIvaPorMesQuery(int? Anio = null) : IRequest<List<IvaPorMesDto>>;

public class GetIvaPorMesQueryHandler : IRequestHandler<GetIvaPorMesQuery, List<IvaPorMesDto>>
{
    private readonly IReporteFacturacionRepository _repo;
    public GetIvaPorMesQueryHandler(IReporteFacturacionRepository repo) => _repo = repo;

    public async Task<List<IvaPorMesDto>> Handle(GetIvaPorMesQuery request, CancellationToken ct)
    {
        var anio = request.Anio ?? DateTime.UtcNow.Year;
        var data = await _repo.GetIvaPorMesAsync(anio);
        return data.Select(d => new IvaPorMesDto(
            d.Anio, d.Mes,
            new DateTime(d.Anio, d.Mes, 1).ToString("MMMM", new System.Globalization.CultureInfo("es-EC")),
            d.Base0, d.Base15, d.TotalIva, d.ImporteTotal))
            .ToList();
    }
}
