using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Kits;

public record UpdateKitCommand(
    long Id,
    string Insumo,
    string Cantidad,
    string Estado,
    string Observacion,
    string Marca,
    string Serie,
    string Modelo) : IRequest<bool>;

public class UpdateKitCommandHandler : IRequestHandler<UpdateKitCommand, bool>
{
    private readonly IKitRepository _kitRepository;

    public UpdateKitCommandHandler(IKitRepository kitRepository)
    {
        _kitRepository = kitRepository;
    }

    public async Task<bool> Handle(UpdateKitCommand request, CancellationToken cancellationToken)
    {
        var kit = await _kitRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Kit con Id {request.Id} no encontrado.");

        kit.Insumo = request.Insumo;
        kit.Cantidad = request.Cantidad;
        kit.Estado = request.Estado;
        kit.Observacion = request.Observacion;
        kit.Marca = request.Marca;
        kit.Serie = request.Serie;
        kit.Modelo = request.Modelo;
        kit.UpdatedAt = DateTime.UtcNow;

        await _kitRepository.UpdateAsync(kit);
        return true;
    }
}
