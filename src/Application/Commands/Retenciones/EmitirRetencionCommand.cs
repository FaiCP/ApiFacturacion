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
    private readonly IComprobanteValidator _validator;
    private readonly IFirmaDigitalService _firmaService;
    private readonly ISRIService _sriService;

    public EmitirRetencionCommandHandler(
        IRetencionRepository retRepo, IEmisorRepository emisorRepo,
        IConfiguracionSRIRepository configRepo, IXmlRetencionService xmlService,
        IComprobanteValidator validator,
        IFirmaDigitalService firmaService, ISRIService sriService)
    {
        _retRepo = retRepo; _emisorRepo = emisorRepo; _configRepo = configRepo;
        _xmlService = xmlService; _validator = validator; _firmaService = firmaService; _sriService = sriService;
    }

    public async Task<EmitirRetencionResult> Handle(EmitirRetencionCommand request, CancellationToken cancellationToken)
    {
        var ret = await _retRepo.GetWithDetallesAsync(request.RetencionId)
            ?? throw new NotFoundException($"Retención {request.RetencionId} no encontrada.");

        if (ret.Estado == EstadoSRI.ANULADA) throw new DomainException("No se puede emitir una retención anulada.");
        if (ret.Estado == EstadoSRI.AUTORIZADA) return new EmitirRetencionResult(EstadoSRI.AUTORIZADA, ret.NumeroAutorizacion, null, "Retención ya autorizada.");

        var emisor = await _emisorRepo.GetByIdAsync(ret.EmisorId) ?? throw new NotFoundException("Emisor no encontrado.");
        ret.Emisor = emisor;

        var config = await _configRepo.GetActivaPorEmisorAsync(emisor.Id)
            ?? throw new NotFoundException("No hay configuración SRI activa para este emisor.");

        if (config.FechaVencimientoCert.HasValue && config.FechaVencimientoCert.Value < DateTime.UtcNow)
            throw new DomainException($"Certificado digital venció el {config.FechaVencimientoCert.Value:dd/MM/yyyy}. Actualice el certificado en Configuración SRI.");

        if (emisor.Ambiente != config.Ambiente)
            throw new DomainException($"Conflicto de ambiente: emisor en {emisor.Ambiente} pero configuración SRI en {config.Ambiente}. Verifique la configuración.");

        var xml = _xmlService.GenerarXml(ret);

        var validacion = _validator.Validar(xml);
        if (!validacion.EsValido)
        {
            ret.Estado = EstadoSRI.RECHAZADA;
            ret.MotivoRechazo = validacion.MensajeConsolidado();
            ret.UpdatedAt = DateTime.UtcNow;
            await _retRepo.UpdateAsync(ret);
            return new EmitirRetencionResult(EstadoSRI.RECHAZADA, null, ret.MotivoRechazo, "Comprobante inválido: no se envió al SRI.");
        }

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
