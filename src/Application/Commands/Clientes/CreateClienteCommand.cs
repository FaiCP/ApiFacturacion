using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Clientes;

public record CreateClienteCommand(
    TipoIdentificacion TipoIdentificacion,
    string NumeroIdentificacion,
    string RazonSocial,
    string? Email,
    string? Telefono,
    string? Direccion) : IRequest<long>;

public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, long>
{
    private readonly IClienteRepository _clienteRepository;

    public CreateClienteCommandHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<long> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = new Domain.Entities.Cliente
        {
            TipoIdentificacion = request.TipoIdentificacion,
            NumeroIdentificacion = request.NumeroIdentificacion,
            RazonSocial = request.RazonSocial,
            Email = request.Email,
            Telefono = request.Telefono,
            Direccion = request.Direccion,
            Borrado = false
        };

        var result = await _clienteRepository.AddAsync(cliente);
        return result.Id;
    }
}
