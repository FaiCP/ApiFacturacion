using Domain.Enums;
using Domain.Interfaces;
using MediatR;
using System.Security.Cryptography.X509Certificates;

namespace Application.Commands.ConfiguracionSRI;

public record SaveConfiguracionSRICommand(
    long EmisorId,
    string CertificadoBase64,
    string PasswordCertificado,
    AmbienteSRI Ambiente) : IRequest<long>;

public class SaveConfiguracionSRICommandHandler : IRequestHandler<SaveConfiguracionSRICommand, long>
{
    private readonly IConfiguracionSRIRepository _configRepo;
    private readonly IEmisorRepository _emisorRepo;

    public SaveConfiguracionSRICommandHandler(IConfiguracionSRIRepository configRepo, IEmisorRepository emisorRepo)
    {
        _configRepo = configRepo;
        _emisorRepo = emisorRepo;
    }

    public async Task<long> Handle(SaveConfiguracionSRICommand request, CancellationToken cancellationToken)
    {
        _ = await _emisorRepo.GetByIdAsync(request.EmisorId)
            ?? throw new Domain.Exceptions.NotFoundException($"Emisor {request.EmisorId} no encontrado.");

        // Validar certificado antes de guardar
        DateTime? vencimiento = null;
        try
        {
            var certBytes = Convert.FromBase64String(request.CertificadoBase64);
            using var cert = new X509Certificate2(certBytes, request.PasswordCertificado);
            vencimiento = cert.NotAfter;
        }
        catch
        {
            throw new Domain.Exceptions.DomainException("El certificado .p12 no es válido o la contraseña es incorrecta.");
        }

        // Desactivar configuración anterior de este emisor (no afecta otros emisores)
        var anterior = await _configRepo.GetActivaPorEmisorAsync(request.EmisorId);
        if (anterior != null)
        {
            anterior.Activo = false;
            anterior.UpdatedAt = DateTime.UtcNow;
            await _configRepo.UpdateAsync(anterior);
        }

        var config = new Domain.Entities.ConfiguracionSRI
        {
            EmisorId = request.EmisorId,
            CertificadoBase64 = request.CertificadoBase64,
            PasswordCertificado = request.PasswordCertificado,
            Ambiente = request.Ambiente,
            Activo = true,
            FechaVencimientoCert = vencimiento,
            Borrado = false
        };

        var result = await _configRepo.AddAsync(config);
        return result.Id;
    }
}
