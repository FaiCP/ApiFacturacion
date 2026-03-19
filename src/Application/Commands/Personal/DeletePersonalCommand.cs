using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Personal;

public record DeletePersonalCommand(List<long> Ids) : IRequest<bool>;

public class DeletePersonalCommandHandler : IRequestHandler<DeletePersonalCommand, bool>
{
    private readonly IPersonalRepository _personalRepository;

    public DeletePersonalCommandHandler(IPersonalRepository personalRepository)
    {
        _personalRepository = personalRepository;
    }

    public async Task<bool> Handle(DeletePersonalCommand request, CancellationToken cancellationToken)
    {
        foreach (var id in request.Ids)
        {
            var persona = await _personalRepository.GetByIdAsync(id);
            if (persona != null)
            {
                persona.Borrado = true;
                persona.UpdatedAt = DateTime.UtcNow;
                await _personalRepository.UpdateAsync(persona);
            }
        }
        return true;
    }
}
