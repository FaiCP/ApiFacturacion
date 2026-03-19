using Application.Common;
using Application.DTOs.Custodios;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Custodios;

public record GetCustodiosQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<CustodioDto>>;

public class GetCustodiosQueryHandler : IRequestHandler<GetCustodiosQuery, PaginatedResponse<CustodioDto>>
{
    private readonly ICustodioRepository _custodioRepository;
    private readonly IMapper _mapper;

    public GetCustodiosQueryHandler(ICustodioRepository custodioRepository, IMapper mapper)
    {
        _custodioRepository = custodioRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<CustodioDto>> Handle(GetCustodiosQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = query.Pagina <= 0 ? 1 : query.Pagina;
        var items = await _custodioRepository.SearchAsync(query.Busqueda, pageNumber, query.Cantidad);
        var total = await _custodioRepository.CountSearchAsync(query.Busqueda);

        return new PaginatedResponse<CustodioDto>
        {
            Items = _mapper.Map<List<CustodioDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
