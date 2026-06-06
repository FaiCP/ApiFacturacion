using Application.DTOs.Reportes;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reportes;

public record GetFacturasPorEstadoQuery : IRequest<List<FacturasPorEstadoDto>>;

public class GetFacturasPorEstadoQueryHandler : IRequestHandler<GetFacturasPorEstadoQuery, List<FacturasPorEstadoDto>>
{
    private readonly IReporteFacturacionRepository _repo;
    public GetFacturasPorEstadoQueryHandler(IReporteFacturacionRepository repo) => _repo = repo;

    public async Task<List<FacturasPorEstadoDto>> Handle(GetFacturasPorEstadoQuery request, CancellationToken ct)
    {
        var data = await _repo.GetFacturasPorEstadoAsync();
        return data.Select(d => new FacturasPorEstadoDto(d.Estado, d.Cantidad, d.TotalMonto)).ToList();
    }
}
