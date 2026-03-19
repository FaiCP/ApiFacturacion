using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Kits;

public record CreateKitCommand(
    string Insumo,
    string Cantidad,
    string Estado,
    string Observacion,
    string Marca,
    string Serie,
    string Modelo) : IRequest<long>;

public class CreateKitCommandHandler : IRequestHandler<CreateKitCommand, long>
{
    private readonly IKitRepository _kitRepository;

    public CreateKitCommandHandler(IKitRepository kitRepository)
    {
        _kitRepository = kitRepository;
    }

    public async Task<long> Handle(CreateKitCommand request, CancellationToken cancellationToken)
    {
        var kit = new Kit
        {
            Insumo = request.Insumo,
            Cantidad = request.Cantidad,
            Estado = request.Estado,
            Observacion = request.Observacion,
            Marca = request.Marca,
            Serie = request.Serie,
            Modelo = request.Modelo,
            Borrado = false
        };

        var result = await _kitRepository.AddAsync(kit);
        return result.Id;
    }
}
