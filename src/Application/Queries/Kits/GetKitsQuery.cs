using Application.Common;
using Application.DTOs.Kits;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Kits;

public record GetKitsQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<KitDto>>;

public class GetKitsQueryHandler : IRequestHandler<GetKitsQuery, PaginatedResponse<KitDto>>
{
    private readonly IKitRepository _kitRepository;
    private readonly IMapper _mapper;

    public GetKitsQueryHandler(IKitRepository kitRepository, IMapper mapper)
    {
        _kitRepository = kitRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<KitDto>> Handle(GetKitsQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = query.Pagina <= 0 ? 1 : query.Pagina;
        var items = await _kitRepository.SearchAsync(query.Busqueda, pageNumber, query.Cantidad);
        var total = await _kitRepository.CountSearchAsync(query.Busqueda);

        return new PaginatedResponse<KitDto>
        {
            Items = _mapper.Map<List<KitDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
