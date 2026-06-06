using Application.Common;
using Application.DTOs.Retenciones;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Retenciones;

public record GetRetencionesQuery(int Cantidad, int Pagina, EstadoSRI? Estado, long? FacturaId) : IRequest<PaginatedResponse<RetencionDto>>;

public class GetRetencionesQueryHandler : IRequestHandler<GetRetencionesQuery, PaginatedResponse<RetencionDto>>
{
    private readonly IRetencionRepository _retRepo;
    private readonly IMapper _mapper;
    public GetRetencionesQueryHandler(IRetencionRepository retRepo, IMapper mapper) { _retRepo = retRepo; _mapper = mapper; }

    public async Task<PaginatedResponse<RetencionDto>> Handle(GetRetencionesQuery query, CancellationToken ct)
    {
        var (items, total) = await _retRepo.GetPagedAsync(query.Pagina, query.Cantidad, query.Estado, query.FacturaId);
        return new PaginatedResponse<RetencionDto>
        {
            Items = _mapper.Map<List<RetencionDto>>(items),
            TotalCount = total, PageNumber = query.Pagina, PageSize = query.Cantidad
        };
    }
}
