using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Clientes;

public record UpdateClienteCommand(
    long Id,
    string RazonSocial,
    string? Email,
    string? Telefono,
    string? Direccion) : IRequest<bool>;

public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, bool>
{
    private readonly IClienteRepository _clienteRepository;

    public UpdateClienteCommandHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<bool> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Cliente con Id {request.Id} no encontrado.");

        cliente.RazonSocial = request.RazonSocial;
        cliente.Email = request.Email;
        cliente.Telefono = request.Telefono;
        cliente.Direccion = request.Direccion;
        cliente.UpdatedAt = DateTime.UtcNow;

        await _clienteRepository.UpdateAsync(cliente);
        return true;
    }
}
