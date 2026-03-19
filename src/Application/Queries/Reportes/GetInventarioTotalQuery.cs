using Application.DTOs.Reportes;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reportes;

public record GetInventarioTotalQuery : IRequest<List<ReporteInventarioDto>>;

public class GetInventarioTotalQueryHandler : IRequestHandler<GetInventarioTotalQuery, List<ReporteInventarioDto>>
{
    private readonly IHardwareRepository _hardwareRepository;

    public GetInventarioTotalQueryHandler(IHardwareRepository hardwareRepository)
    {
        _hardwareRepository = hardwareRepository;
    }

    public async Task<List<ReporteInventarioDto>> Handle(GetInventarioTotalQuery request, CancellationToken cancellationToken)
    {
        var all = await _hardwareRepository.GetAllAsync();
        return all
            .GroupBy(h => h.NombreDispositivo)
            .Select(g => new ReporteInventarioDto(g.Key, g.Count()))
            .ToList();
    }
}
