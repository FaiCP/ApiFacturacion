using Application.Common;
using Application.DTOs.Facturas;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Facturas;

public record GetFacturasQuery(
    int Cantidad,
    int Pagina,
    EstadoSRI? Estado,
    DateTime? Desde,
    DateTime? Hasta,
    long? ClienteId) : IRequest<PaginatedResponse<FacturaResumenDto>>;

public class GetFacturasQueryHandler : IRequestHandler<GetFacturasQuery, PaginatedResponse<FacturaResumenDto>>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IMapper _mapper;

    public GetFacturasQueryHandler(IFacturaRepository facturaRepository, IMapper mapper)
    {
        _facturaRepository = facturaRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<FacturaResumenDto>> Handle(GetFacturasQuery query, CancellationToken cancellationToken)
    {
        var (items, total) = await _facturaRepository.GetPagedAsync(
            query.Pagina, query.Cantidad, query.Estado, query.Desde, query.Hasta, query.ClienteId);

        return new PaginatedResponse<FacturaResumenDto>
        {
            Items = _mapper.Map<List<FacturaResumenDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
