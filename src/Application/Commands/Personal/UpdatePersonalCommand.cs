using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Personal;

public record UpdatePersonalCommand(
    long Id,
    string Nombre,
    string Cedula,
    string Cargo,
    string Email,
    string? TempPass,
    DateTime? Fecha) : IRequest<bool>;

public class UpdatePersonalCommandHandler : IRequestHandler<UpdatePersonalCommand, bool>
{
    private readonly IPersonalRepository _personalRepository;

    public UpdatePersonalCommandHandler(IPersonalRepository personalRepository)
    {
        _personalRepository = personalRepository;
    }

    public async Task<bool> Handle(UpdatePersonalCommand request, CancellationToken cancellationToken)
    {
        var persona = await _personalRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Personal con Id {request.Id} no encontrado.");

        persona.Nombre = request.Nombre;
        persona.Cedula = request.Cedula;
        persona.Cargo = request.Cargo;
        persona.Email = request.Email;
        persona.TempPass = request.TempPass;
        persona.Fecha = request.Fecha;
        persona.UpdatedAt = DateTime.UtcNow;

        await _personalRepository.UpdateAsync(persona);
        return true;
    }
}
