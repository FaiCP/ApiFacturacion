using Application.DTOs.Clientes;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Clientes;

public record GetClienteByIdQuery(long Id) : IRequest<ClienteDto>;

public class GetClienteByIdQueryHandler : IRequestHandler<GetClienteByIdQuery, ClienteDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IMapper _mapper;

    public GetClienteByIdQueryHandler(IClienteRepository clienteRepository, IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _mapper = mapper;
    }

    public async Task<ClienteDto> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Cliente con Id {request.Id} no encontrado.");
        return _mapper.Map<ClienteDto>(cliente);
    }
}
