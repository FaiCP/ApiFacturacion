using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Reportes;

public record GetAtsSRIQuery(int Anio, int Mes) : IRequest<byte[]>;

public class GetAtsSRIQueryHandler : IRequestHandler<GetAtsSRIQuery, byte[]>
{
    private readonly IReporteFacturacionRepository _repo;
    private readonly IAtsExcelService _atsService;

    public GetAtsSRIQueryHandler(IReporteFacturacionRepository repo, IAtsExcelService atsService)
    {
        _repo = repo;
        _atsService = atsService;
    }

    public async Task<byte[]> Handle(GetAtsSRIQuery request, CancellationToken ct)
    {
        var facturas = await _repo.GetFacturasParaAtsAsync(request.Anio, request.Mes);
        var retenciones = await _repo.GetRetencionesParaAtsAsync(request.Anio, request.Mes);
        return _atsService.GenerarAts(facturas, retenciones, request.Anio, request.Mes);
    }
}
