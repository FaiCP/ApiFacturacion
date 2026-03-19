using Application.Common;
using Application.DTOs.GestionActivos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.GestionActivos;

public record GetGestionActivosQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<GestionActivoDto>>;

public class GetGestionActivosQueryHandler : IRequestHandler<GetGestionActivosQuery, PaginatedResponse<GestionActivoDto>>
{
    private readonly IGestionActivoRepository _gestionActivoRepository;
    private readonly IMapper _mapper;

    public GetGestionActivosQueryHandler(IGestionActivoRepository gestionActivoRepository, IMapper mapper)
    {
        _gestionActivoRepository = gestionActivoRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<GestionActivoDto>> Handle(GetGestionActivosQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = query.Pagina <= 0 ? 1 : query.Pagina;
        var items = await _gestionActivoRepository.SearchAsync(query.Busqueda, pageNumber, query.Cantidad);
        var total = await _gestionActivoRepository.CountSearchAsync(query.Busqueda);

        return new PaginatedResponse<GestionActivoDto>
        {
            Items = _mapper.Map<List<GestionActivoDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
