using Application.DTOs.Reportes;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reportes;

public record GetTopClientesQuery(int Top = 10, int? Anio = null) : IRequest<List<TopClienteDto>>;

public class GetTopClientesQueryHandler : IRequestHandler<GetTopClientesQuery, List<TopClienteDto>>
{
    private readonly IReporteFacturacionRepository _repo;
    public GetTopClientesQueryHandler(IReporteFacturacionRepository repo) => _repo = repo;

    public async Task<List<TopClienteDto>> Handle(GetTopClientesQuery request, CancellationToken ct)
    {
        var data = await _repo.GetTopClientesAsync(request.Top, request.Anio);
        return data.Select(d => new TopClienteDto(d.Identificacion, d.RazonSocial, d.TotalFacturas, d.TotalFacturado)).ToList();
    }
}
