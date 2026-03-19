using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Custodios;

public record UpdateCustodioCommand(
    long Id,
    long IdDepartamento,
    string CedulaEmpleado,
    string CargoEmpleado,
    string NombreEmpleado) : IRequest<bool>;

public class UpdateCustodioCommandHandler : IRequestHandler<UpdateCustodioCommand, bool>
{
    private readonly ICustodioRepository _custodioRepository;

    public UpdateCustodioCommandHandler(ICustodioRepository custodioRepository)
    {
        _custodioRepository = custodioRepository;
    }

    public async Task<bool> Handle(UpdateCustodioCommand request, CancellationToken cancellationToken)
    {
        var custodio = await _custodioRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Custodio con Id {request.Id} no encontrado.");

        custodio.Nombre = request.NombreEmpleado;
        custodio.Cedula = request.CedulaEmpleado;
        custodio.Cargo = request.CargoEmpleado;
        custodio.IdDepartamento = request.IdDepartamento;
        custodio.UpdatedAt = DateTime.UtcNow;

        await _custodioRepository.UpdateAsync(custodio);
        return true;
    }
}
