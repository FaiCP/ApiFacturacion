using Application.Common;
using Application.DTOs.Productos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Productos;

public record GetProductosQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<ProductoDto>>;

public class GetProductosQueryHandler : IRequestHandler<GetProductosQuery, PaginatedResponse<ProductoDto>>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IMapper _mapper;

    public GetProductosQueryHandler(IProductoRepository productoRepository, IMapper mapper)
    {
        _productoRepository = productoRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<ProductoDto>> Handle(GetProductosQuery query, CancellationToken cancellationToken)
    {
        var (items, total) = await _productoRepository.GetPagedAsync(query.Pagina, query.Cantidad, query.Busqueda);
        return new PaginatedResponse<ProductoDto>
        {
            Items = _mapper.Map<List<ProductoDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
