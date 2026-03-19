using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Hardware;

public record CreateHardwareCommand(
    string Ubicacion,
    string Descripcion,
    string NombreDispositivo,
    string Marca,
    string Modelo,
    string CodigoCne,
    string IdEquipo,
    string Estado,
    string Ram,
    string Rom,
    string Procesador,
    double? Valor) : IRequest<long>;

public class CreateHardwareCommandHandler : IRequestHandler<CreateHardwareCommand, long>
{
    private readonly IHardwareRepository _hardwareRepository;
    private readonly IGenericRepository<CaracteristicaComputadora> _caracteristicaRepository;

    public CreateHardwareCommandHandler(
        IHardwareRepository hardwareRepository,
        IGenericRepository<CaracteristicaComputadora> caracteristicaRepository)
    {
        _hardwareRepository = hardwareRepository;
        _caracteristicaRepository = caracteristicaRepository;
    }

    public async Task<long> Handle(CreateHardwareCommand request, CancellationToken cancellationToken)
    {
        var hardware = new Domain.Entities.Hardware
        {
            IdEquipo = request.IdEquipo,
            Marca = request.Marca,
            Modelo = request.Modelo,
            Estado = request.Estado,
            Ubicacion = request.Ubicacion,
            CodigoCne = request.CodigoCne,
            NombreDispositivo = request.NombreDispositivo,
            Observacion = request.Descripcion,
            Valor = request.Valor.HasValue ? Convert.ToDecimal(request.Valor.Value) : null,
            Borrado = false
        };

        var result = await _hardwareRepository.AddAsync(hardware);

        if (!string.IsNullOrWhiteSpace(request.Ram) ||
            !string.IsNullOrWhiteSpace(request.Rom) ||
            !string.IsNullOrWhiteSpace(request.Procesador))
        {
            var caracteristica = new CaracteristicaComputadora
            {
                IdEquipo = request.IdEquipo,
                Ram = request.Ram,
                Rom = request.Rom,
                Procesador = request.Procesador,
                HardwareId = result.Id
            };

            await _caracteristicaRepository.AddAsync(caracteristica);
        }

        return result.Id;
    }
}
