using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Emisor;

public record UpdateEmisorCommand(
    long Id,
    string RazonSocial,
    string? NombreComercial,
    string Direccion,
    string? Telefono,
    string? Email,
    bool ObligadoContabilidad,
    AmbienteSRI Ambiente,
    string SerieEstablecimiento,
    string SeriePuntoEmision,
    string? LogoBase64) : IRequest<bool>;

public class UpdateEmisorCommandHandler : IRequestHandler<UpdateEmisorCommand, bool>
{
    private readonly IEmisorRepository _emisorRepository;

    public UpdateEmisorCommandHandler(IEmisorRepository emisorRepository)
    {
        _emisorRepository = emisorRepository;
    }

    public async Task<bool> Handle(UpdateEmisorCommand request, CancellationToken cancellationToken)
    {
        var emisor = await _emisorRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Emisor con Id {request.Id} no encontrado.");

        emisor.RazonSocial = request.RazonSocial;
        emisor.NombreComercial = request.NombreComercial;
        emisor.Direccion = request.Direccion;
        emisor.Telefono = request.Telefono;
        emisor.Email = request.Email;
        emisor.ObligadoContabilidad = request.ObligadoContabilidad;
        emisor.Ambiente = request.Ambiente;
        emisor.SerieEstablecimiento = request.SerieEstablecimiento;
        emisor.SeriePuntoEmision = request.SeriePuntoEmision;
        emisor.LogoBase64 = request.LogoBase64;
        emisor.UpdatedAt = DateTime.UtcNow;

        await _emisorRepository.UpdateAsync(emisor);
        return true;
    }
}
