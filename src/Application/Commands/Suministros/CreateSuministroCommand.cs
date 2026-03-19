using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Suministros;

public record CreateSuministroCommand(
    string IdEquipo,
    string TipoSuministro,
    string IdEquipoAsignado,
    DateTime? FechaRetiro) : IRequest<long>;

public class CreateSuministroCommandHandler : IRequestHandler<CreateSuministroCommand, long>
{
    private readonly ISuministroRepository _suministroRepository;

    public CreateSuministroCommandHandler(ISuministroRepository suministroRepository)
    {
        _suministroRepository = suministroRepository;
    }

    public async Task<long> Handle(CreateSuministroCommand request, CancellationToken cancellationToken)
    {
        var suministro = new Suministro
        {
            IdEquipo = request.IdEquipo,
            TipoSuministro = request.TipoSuministro,
            IdEquipoAsignado = request.IdEquipoAsignado,
            FechaRetiro = request.FechaRetiro,
            Borrado = false
        };

        var result = await _suministroRepository.AddAsync(suministro);
        return result.Id;
    }
}
