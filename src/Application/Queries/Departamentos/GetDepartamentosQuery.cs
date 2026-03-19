using Application.Common;
using Application.DTOs.Departamentos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Departamentos;

public record GetDepartamentosQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<DepartamentoDto>>;

public class GetDepartamentosQueryHandler : IRequestHandler<GetDepartamentosQuery, PaginatedResponse<DepartamentoDto>>
{
    private readonly IDepartamentoRepository _departamentoRepository;
    private readonly IMapper _mapper;

    public GetDepartamentosQueryHandler(IDepartamentoRepository departamentoRepository, IMapper mapper)
    {
        _departamentoRepository = departamentoRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<DepartamentoDto>> Handle(GetDepartamentosQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = query.Pagina <= 0 ? 1 : query.Pagina;
        var items = await _departamentoRepository.SearchAsync(query.Busqueda, pageNumber, query.Cantidad);
        var total = await _departamentoRepository.CountSearchAsync(query.Busqueda);

        return new PaginatedResponse<DepartamentoDto>
        {
            Items = _mapper.Map<List<DepartamentoDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
