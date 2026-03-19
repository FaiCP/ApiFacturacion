using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Custodios;

public record DeleteCustodioCommand(List<long> Ids) : IRequest<bool>;

public class DeleteCustodioCommandHandler : IRequestHandler<DeleteCustodioCommand, bool>
{
    private readonly ICustodioRepository _custodioRepository;

    public DeleteCustodioCommandHandler(ICustodioRepository custodioRepository)
    {
        _custodioRepository = custodioRepository;
    }

    public async Task<bool> Handle(DeleteCustodioCommand request, CancellationToken cancellationToken)
    {
        foreach (var id in request.Ids)
        {
            var custodio = await _custodioRepository.GetByIdAsync(id);
            if (custodio != null)
            {
                custodio.Borrado = true;
                custodio.UpdatedAt = DateTime.UtcNow;
                await _custodioRepository.UpdateAsync(custodio);
            }
        }
        return true;
    }
}
