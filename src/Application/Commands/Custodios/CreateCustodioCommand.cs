using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Custodios;

public record CreateCustodioCommand(
    string Nombre,
    string Cargo,
    string Cedula,
    long? IdDepartamento) : IRequest<long>;

public class CreateCustodioCommandHandler : IRequestHandler<CreateCustodioCommand, long>
{
    private readonly ICustodioRepository _custodioRepository;

    public CreateCustodioCommandHandler(ICustodioRepository custodioRepository)
    {
        _custodioRepository = custodioRepository;
    }

    public async Task<long> Handle(CreateCustodioCommand request, CancellationToken cancellationToken)
    {
        var custodio = new Custodio
        {
            Nombre = request.Nombre,
            Cargo = request.Cargo,
            Cedula = request.Cedula,
            IdDepartamento = request.IdDepartamento,
            Borrado = false
        };

        var result = await _custodioRepository.AddAsync(custodio);
        return result.Id;
    }
}
