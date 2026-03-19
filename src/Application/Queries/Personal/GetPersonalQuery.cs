using Application.Common;
using Application.DTOs.Personal;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Personal;

public record GetPersonalQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<PersonalDto>>;

public class GetPersonalQueryHandler : IRequestHandler<GetPersonalQuery, PaginatedResponse<PersonalDto>>
{
    private readonly IPersonalRepository _personalRepository;
    private readonly IMapper _mapper;

    public GetPersonalQueryHandler(IPersonalRepository personalRepository, IMapper mapper)
    {
        _personalRepository = personalRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<PersonalDto>> Handle(GetPersonalQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = query.Pagina <= 0 ? 1 : query.Pagina;
        var items = await _personalRepository.SearchAsync(query.Busqueda, pageNumber, query.Cantidad);
        var total = await _personalRepository.CountSearchAsync(query.Busqueda);

        return new PaginatedResponse<PersonalDto>
        {
            Items = _mapper.Map<List<PersonalDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
