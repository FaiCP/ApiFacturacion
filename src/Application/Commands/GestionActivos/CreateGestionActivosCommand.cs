using Application.DTOs.GestionActivos;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.GestionActivos;

public record CreateGestionActivosCommand(List<AsignacionEquipoDto> Asignaciones) : IRequest<List<long?>>;

public class CreateGestionActivosCommandHandler : IRequestHandler<CreateGestionActivosCommand, List<long?>>
{
    private readonly IGestionActivoRepository _gestionActivoRepository;

    public CreateGestionActivosCommandHandler(IGestionActivoRepository gestionActivoRepository)
    {
        _gestionActivoRepository = gestionActivoRepository;
    }

    public async Task<List<long?>> Handle(CreateGestionActivosCommand request, CancellationToken cancellationToken)
    {
        var ids = new List<long?>();

        foreach (var asignacion in request.Asignaciones)
        {
            var gestionActivo = new GestionActivo
            {
                IdEquipo = asignacion.IdEquipo,
                IdCustodio = asignacion.IdCustodio,
                FechaAsignacion = DateTime.UtcNow,
                Borrado = false
            };

            var result = await _gestionActivoRepository.AddAsync(gestionActivo);
            ids.Add(result.Id);
        }

        return ids;
    }
}
