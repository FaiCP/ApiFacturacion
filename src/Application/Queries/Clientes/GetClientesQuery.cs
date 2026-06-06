using Application.Common;
using Application.DTOs.Clientes;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Clientes;

public record GetClientesQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<ClienteDto>>;

public class GetClientesQueryHandler : IRequestHandler<GetClientesQuery, PaginatedResponse<ClienteDto>>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMapper _mapper;

    public GetClientesQueryHandler(IClienteRepository clienteRepository, IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<ClienteDto>> Handle(GetClientesQuery query, CancellationToken cancellationToken)
    {
        var (items, total) = await _clienteRepository.GetPagedAsync(query.Pagina, query.Cantidad, query.Busqueda);
        return new PaginatedResponse<ClienteDto>
        {
            Items = _mapper.Map<List<ClienteDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
