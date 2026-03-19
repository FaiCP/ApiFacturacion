using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Kits;

public record DeleteKitCommand(List<long> Ids) : IRequest<bool>;

public class DeleteKitCommandHandler : IRequestHandler<DeleteKitCommand, bool>
{
    private readonly IKitRepository _kitRepository;

    public DeleteKitCommandHandler(IKitRepository kitRepository)
    {
        _kitRepository = kitRepository;
    }

    public async Task<bool> Handle(DeleteKitCommand request, CancellationToken cancellationToken)
    {
        foreach (var id in request.Ids)
        {
            var kit = await _kitRepository.GetByIdAsync(id);
            if (kit != null)
            {
                kit.Borrado = true;
                kit.UpdatedAt = DateTime.UtcNow;
                await _kitRepository.UpdateAsync(kit);
            }
        }
        return true;
    }
}
