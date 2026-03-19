using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.GestionActivos;

public record UpdateGestionActivoCommand(
    long Id,
    string? IdEquipo,
    long? IdCustodio,
    DateTime? FechaAsignacion,
    DateTime? FechaDevolucion) : IRequest<bool>;

public class UpdateGestionActivoCommandHandler : IRequestHandler<UpdateGestionActivoCommand, bool>
{
    private readonly IGestionActivoRepository _gestionActivoRepository;

    public UpdateGestionActivoCommandHandler(IGestionActivoRepository gestionActivoRepository)
    {
        _gestionActivoRepository = gestionActivoRepository;
    }

    public async Task<bool> Handle(UpdateGestionActivoCommand request, CancellationToken cancellationToken)
    {
        var gestionActivo = await _gestionActivoRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"GestionActivo con Id {request.Id} no encontrado.");

        gestionActivo.IdEquipo = request.IdEquipo;
        gestionActivo.IdCustodio = request.IdCustodio;
        gestionActivo.FechaAsignacion = request.FechaAsignacion;
        gestionActivo.FechaDevolucion = request.FechaDevolucion;
        gestionActivo.UpdatedAt = DateTime.UtcNow;

        await _gestionActivoRepository.UpdateAsync(gestionActivo);
        return true;
    }
}
