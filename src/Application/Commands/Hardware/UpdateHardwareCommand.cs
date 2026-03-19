using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Hardware;

public record UpdateHardwareCommand(
    long Id,
    string IdEquipo,
    string Descripcion,
    string Marca,
    string Modelo,
    string Estado,
    string Ubicacion,
    string CodigoCne,
    string NombreDispositivo,
    double? Valor) : IRequest<bool>;

public class UpdateHardwareCommandHandler : IRequestHandler<UpdateHardwareCommand, bool>
{
    private readonly IHardwareRepository _hardwareRepository;

    public UpdateHardwareCommandHandler(IHardwareRepository hardwareRepository)
    {
        _hardwareRepository = hardwareRepository;
    }

    public async Task<bool> Handle(UpdateHardwareCommand request, CancellationToken cancellationToken)
    {
        var hardware = await _hardwareRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Hardware con Id {request.Id} no encontrado.");

        hardware.IdEquipo = request.IdEquipo;
        hardware.Observacion = request.Descripcion;
        hardware.Marca = request.Marca;
        hardware.Modelo = request.Modelo;
        hardware.Estado = request.Estado;
        hardware.Ubicacion = request.Ubicacion;
        hardware.CodigoCne = request.CodigoCne;
        hardware.NombreDispositivo = request.NombreDispositivo;
        hardware.Valor = request.Valor.HasValue ? Convert.ToDecimal(request.Valor.Value) : null;
        hardware.UpdatedAt = DateTime.UtcNow;

        await _hardwareRepository.UpdateAsync(hardware);
        return true;
    }
}
