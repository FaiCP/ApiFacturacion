using Domain.Interfaces;
using MediatR;

namespace Application.Commands.GestionActivos;

public record DeleteGestionActivoCommand(List<long> Ids) : IRequest<bool>;

public class DeleteGestionActivoCommandHandler : IRequestHandler<DeleteGestionActivoCommand, bool>
{
    private readonly IGestionActivoRepository _gestionActivoRepository;

    public DeleteGestionActivoCommandHandler(IGestionActivoRepository gestionActivoRepository)
    {
        _gestionActivoRepository = gestionActivoRepository;
    }

    public async Task<bool> Handle(DeleteGestionActivoCommand request, CancellationToken cancellationToken)
    {
        foreach (var id in request.Ids)
        {
            var gestionActivo = await _gestionActivoRepository.GetByIdAsync(id);
            if (gestionActivo != null)
            {
                gestionActivo.Borrado = true;
                gestionActivo.UpdatedAt = DateTime.UtcNow;
                await _gestionActivoRepository.UpdateAsync(gestionActivo);
            }
        }
        return true;
    }
}
