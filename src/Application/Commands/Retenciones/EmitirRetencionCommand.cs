using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Retenciones;

public record EmitirRetencionCommand(long RetencionId) : IRequest<EmitirRetencionResult>;
public record EmitirRetencionResult(EstadoSRI Estado, string? NumeroAutorizacion, string? MotivoRechazo, string Mensaje);

public class EmitirRetencionCommandHandler : IRequestHandler<EmitirRetencionCommand, EmitirRetencionResult>
{
    private readonly IRetencionRepository _retRepo;
    private readonly IEmisorRepository _emisorRepo;
    private readonly IConfiguracionSRIRepository _configRepo;
    private readonly IXmlRetencionService _xmlService;
    private readonly IFirmaDigitalService _firmaService;
    private readonly ISRIService _sriService;

    public EmitirRetencionCommandHandler(
        IRetencionRepository retRepo, IEmisorRepository emisorRepo,
        IConfiguracionSRIRepository configRepo, IXmlRetencionService xmlService,
        IFirmaDigitalService firmaService, ISRIService sriService)
    {
        _retRepo = retRepo; _emisorRepo = emisorRepo; _configRepo = configRepo;
        _xmlService = xmlService; _firmaService = firmaService; _sriService = sriService;
    }

    public async Task<EmitirRetencionResult> Handle(EmitirRetencionCommand request, CancellationToken cancellationToken)
    {
        var ret = await _retRepo.GetWithDetallesAsync(request.RetencionId)
            ?? throw new NotFoundException($"Retención {request.RetencionId} no encontrada.");

        if (ret.Estado == EstadoSRI.ANULADA) throw new DomainException("No se puede emitir una retención anulada.");
        if (ret.Estado == EstadoSRI.AUTORIZADA) return new EmitirRetencionResult(EstadoSRI.AUTORIZADA, ret.NumeroAutorizacion, null, "Retención ya autorizada.");

        var config = await _configRepo.GetActivaAsync() ?? throw new NotFoundException("No hay configuración SRI activa.");
        var emisor = await _emisorRepo.GetByIdAsync(ret.EmisorId) ?? throw new NotFoundException("Emisor no encontrado.");
        ret.Emisor = emisor;

        var xml = _xmlService.GenerarXml(ret);
        var xmlFirmado = await _firmaService.FirmarXmlAsync(xml, config.CertificadoBase64!, config.PasswordCertificado!);

        var recepcion = await _sriService.EnviarComprobanteAsync(xmlFirmado, config.Ambiente);
        if (!recepcion.Exitoso)
        {
            ret.Estado = EstadoSRI.RECHAZADA;
            ret.MotivoRechazo = string.Join(" | ", recepcion.Errores);
            ret.UpdatedAt = DateTime.UtcNow;
            await _retRepo.UpdateAsync(ret);
            return new EmitirRetencionResult(EstadoSRI.RECHAZADA, null, ret.MotivoRechazo, "Retención rechazada por el SRI.");
        }

        ret.Estado = EstadoSRI.ENVIADA;
        ret.XmlFirmado = xmlFirmado;
        ret.UpdatedAt = DateTime.UtcNow;
        await _retRepo.UpdateAsync(ret);

        await Task.Delay(2000, cancellationToken);
        var autorizacion = await _sriService.ConsultarAutorizacionAsync(ret.ClaveAcceso!, config.Ambiente);

        ret.Estado = autorizacion.Estado;
        ret.NumeroAutorizacion = autorizacion.NumeroAutorizacion;
        ret.FechaAutorizacion = autorizacion.FechaAutorizacion;
        ret.MotivoRechazo = autorizacion.MotivoRechazo;
        ret.UpdatedAt = DateTime.UtcNow;
        await _retRepo.UpdateAsync(ret);

        return new EmitirRetencionResult(autorizacion.Estado, autorizacion.NumeroAutorizacion, autorizacion.MotivoRechazo,
            autorizacion.Estado == EstadoSRI.AUTORIZADA ? $"Retención autorizada. Nº: {autorizacion.NumeroAutorizacion}" : $"Estado: {autorizacion.Estado}");
    }
}
