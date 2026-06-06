using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Emisor;

public record CreateEmisorCommand(
    string Ruc,
    string RazonSocial,
    string? NombreComercial,
    string Direccion,
    string? Telefono,
    string? Email,
    bool ObligadoContabilidad,
    AmbienteSRI Ambiente,
    string SerieEstablecimiento,
    string SeriePuntoEmision,
    string? LogoBase64) : IRequest<long>;

public class CreateEmisorCommandHandler : IRequestHandler<CreateEmisorCommand, long>
{
    private readonly IEmisorRepository _emisorRepository;

    public CreateEmisorCommandHandler(IEmisorRepository emisorRepository)
    {
        _emisorRepository = emisorRepository;
    }

    public async Task<long> Handle(CreateEmisorCommand request, CancellationToken cancellationToken)
    {
        var emisor = new Domain.Entities.Emisor
        {
            Ruc = request.Ruc,
            RazonSocial = request.RazonSocial,
            NombreComercial = request.NombreComercial,
            Direccion = request.Direccion,
            Telefono = request.Telefono,
            Email = request.Email,
            ObligadoContabilidad = request.ObligadoContabilidad,
            Ambiente = request.Ambiente,
            SerieEstablecimiento = request.SerieEstablecimiento,
            SeriePuntoEmision = request.SeriePuntoEmision,
            LogoBase64 = request.LogoBase64,
            Borrado = false
        };

        var result = await _emisorRepository.AddAsync(emisor);
        return result.Id;
    }
}
