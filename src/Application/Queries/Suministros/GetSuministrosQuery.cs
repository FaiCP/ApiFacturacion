using Application.Common;
using Application.DTOs.Suministros;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Suministros;

public record GetSuministrosQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<SuministroDto>>;

public class GetSuministrosQueryHandler : IRequestHandler<GetSuministrosQuery, PaginatedResponse<SuministroDto>>
{
    private readonly ISuministroRepository _suministroRepository;
    private readonly IMapper _mapper;

    public GetSuministrosQueryHandler(ISuministroRepository suministroRepository, IMapper mapper)
    {
        _suministroRepository = suministroRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<SuministroDto>> Handle(GetSuministrosQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = query.Pagina <= 0 ? 1 : query.Pagina;
        var items = await _suministroRepository.SearchAsync(query.Busqueda, pageNumber, query.Cantidad);
        var total = await _suministroRepository.CountSearchAsync(query.Busqueda);

        return new PaginatedResponse<SuministroDto>
        {
            Items = _mapper.Map<List<SuministroDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
