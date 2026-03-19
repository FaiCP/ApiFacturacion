using Application.Common;
using Application.DTOs.HistorialPrestamos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.HistorialPrestamos;

public record GetHistorialPrestamosQuery(int Cantidad, int Pagina, string Busqueda, int IdCustodio) : IRequest<PaginatedResponse<HistorialPrestamoDto>>;

public class GetHistorialPrestamosQueryHandler : IRequestHandler<GetHistorialPrestamosQuery, PaginatedResponse<HistorialPrestamoDto>>
{
    private readonly IGestionActivoRepository _gestionActivoRepository;
    private readonly IMapper _mapper;

    public GetHistorialPrestamosQueryHandler(IGestionActivoRepository gestionActivoRepository, IMapper mapper)
    {
        _gestionActivoRepository = gestionActivoRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<HistorialPrestamoDto>> Handle(GetHistorialPrestamosQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = query.Pagina <= 0 ? 1 : query.Pagina;
        var items = await _gestionActivoRepository.GetHistorialAsync(query.Busqueda, pageNumber, query.Cantidad, query.IdCustodio);
        var total = await _gestionActivoRepository.CountHistorialAsync(query.Busqueda, query.IdCustodio);

        return new PaginatedResponse<HistorialPrestamoDto>
        {
            Items = _mapper.Map<List<HistorialPrestamoDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
