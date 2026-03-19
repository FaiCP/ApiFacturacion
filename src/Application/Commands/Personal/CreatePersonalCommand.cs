using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Personal;

public record CreatePersonalCommand(
    string Nombre,
    string Cedula,
    string Cargo,
    string Email,
    string? TempPass,
    DateTime? Fecha) : IRequest<long>;

public class CreatePersonalCommandHandler : IRequestHandler<CreatePersonalCommand, long>
{
    private readonly IPersonalRepository _personalRepository;

    public CreatePersonalCommandHandler(IPersonalRepository personalRepository)
    {
        _personalRepository = personalRepository;
    }

    public async Task<long> Handle(CreatePersonalCommand request, CancellationToken cancellationToken)
    {
        var persona = new Persona
        {
            Nombre = request.Nombre,
            Cedula = request.Cedula,
            Cargo = request.Cargo,
            Email = request.Email,
            TempPass = request.TempPass,
            Fecha = request.Fecha,
            Borrado = false
        };

        var result = await _personalRepository.AddAsync(persona);
        return result.Id;
    }
}
