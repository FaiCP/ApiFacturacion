using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Hardware;

public record CreateCaracteristicaCommand(
    string IdEquipo,
    string Ram,
    string Rom,
    string Procesador) : IRequest<bool>;

public class CreateCaracteristicaCommandHandler : IRequestHandler<CreateCaracteristicaCommand, bool>
{
    private readonly IGenericRepository<CaracteristicaComputadora> _caracteristicaRepository;

    public CreateCaracteristicaCommandHandler(IGenericRepository<CaracteristicaComputadora> caracteristicaRepository)
    {
        _caracteristicaRepository = caracteristicaRepository;
    }

    public async Task<bool> Handle(CreateCaracteristicaCommand request, CancellationToken cancellationToken)
    {
        var caracteristica = new CaracteristicaComputadora
        {
            IdEquipo = request.IdEquipo,
            Ram = request.Ram,
            Rom = request.Rom,
            Procesador = request.Procesador
        };

        await _caracteristicaRepository.AddAsync(caracteristica);
        return true;
    }
}
