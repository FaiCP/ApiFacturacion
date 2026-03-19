using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Suministros;

public record UpdateSuministroCommand(
    long Id,
    string IdEquipo,
    string TipoSuministro,
    string IdEquipoAsignado,
    DateTime? FechaRetiro) : IRequest<bool>;

public class UpdateSuministroCommandHandler : IRequestHandler<UpdateSuministroCommand, bool>
{
    private readonly ISuministroRepository _suministroRepository;

    public UpdateSuministroCommandHandler(ISuministroRepository suministroRepository)
    {
        _suministroRepository = suministroRepository;
    }

    public async Task<bool> Handle(UpdateSuministroCommand request, CancellationToken cancellationToken)
    {
        var suministro = await _suministroRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Suministro con Id {request.Id} no encontrado.");

        suministro.IdEquipo = request.IdEquipo;
        suministro.TipoSuministro = request.TipoSuministro;
        suministro.IdEquipoAsignado = request.IdEquipoAsignado;
        suministro.FechaRetiro = request.FechaRetiro;
        suministro.UpdatedAt = DateTime.UtcNow;

        await _suministroRepository.UpdateAsync(suministro);
        return true;
    }
}
