using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.NotasDebito;

public record EmitirNotaDebitoCommand(long NotaDebitoId) : IRequest<EmitirNotaDebitoResult>;
public record EmitirNotaDebitoResult(EstadoSRI Estado, string? NumeroAutorizacion, string? MotivoRechazo, string Mensaje);

public class EmitirNotaDebitoCommandHandler : IRequestHandler<EmitirNotaDebitoCommand, EmitirNotaDebitoResult>
{
    private readonly INotaDebitoRepository _ndRepo;
    private readonly IEmisorRepository _emisorRepo;
    private readonly IConfiguracionSRIRepository _configRepo;
    private readonly IXmlNotaDebitoService _xmlService;
    private readonly IComprobanteValidator _validator;
    private readonly IFirmaDigitalService _firmaService;
    private readonly ISRIService _sriService;

    public EmitirNotaDebitoCommandHandler(
        INotaDebitoRepository ndRepo, IEmisorRepository emisorRepo,
        IConfiguracionSRIRepository configRepo, IXmlNotaDebitoService xmlService,
        IComprobanteValidator validator,
        IFirmaDigitalService firmaService, ISRIService sriService)
    {
        _ndRepo = ndRepo; _emisorRepo = emisorRepo; _configRepo = configRepo;
        _xmlService = xmlService; _validator = validator; _firmaService = firmaService; _sriService = sriService;
    }

    public async Task<EmitirNotaDebitoResult> Handle(EmitirNotaDebitoCommand request, CancellationToken cancellationToken)
    {
        var nd = await _ndRepo.GetWithMotivosAsync(request.NotaDebitoId)
            ?? throw new NotFoundException($"Nota de Débito {request.NotaDebitoId} no encontrada.");

        if (nd.Estado == EstadoSRI.ANULADA) throw new DomainException("No se puede emitir una ND anulada.");
        if (nd.Estado == EstadoSRI.AUTORIZADA) return new EmitirNotaDebitoResult(EstadoSRI.AUTORIZADA, nd.NumeroAutorizacion, null, "ND ya autorizada.");

        var emisor = await _emisorRepo.GetByIdAsync(nd.EmisorId) ?? throw new NotFoundException("Emisor no encontrado.");
        nd.Emisor = emisor;

        var config = await _configRepo.GetActivaPorEmisorAsync(emisor.Id)
            ?? throw new NotFoundException("No hay configuración SRI activa para este emisor.");

        if (config.FechaVencimientoCert.HasValue && config.FechaVencimientoCert.Value < DateTime.UtcNow)
            throw new DomainException($"Certificado digital venció el {config.FechaVencimientoCert.Value:dd/MM/yyyy}. Actualice el certificado en Configuración SRI.");

        if (emisor.Ambiente != config.Ambiente)
            throw new DomainException($"Conflicto de ambiente: emisor en {emisor.Ambiente} pero configuración SRI en {config.Ambiente}. Verifique la configuración.");

        var xml = _xmlService.GenerarXml(nd);

        var validacion = _validator.Validar(xml);
        if (!validacion.EsValido)
        {
            nd.Estado = EstadoSRI.RECHAZADA;
            nd.MotivoRechazo = validacion.MensajeConsolidado();
            nd.UpdatedAt = DateTime.UtcNow;
            await _ndRepo.UpdateAsync(nd);
            return new EmitirNotaDebitoResult(EstadoSRI.RECHAZADA, null, nd.MotivoRechazo, "Comprobante inválido: no se envió al SRI.");
        }

        var xmlFirmado = await _firmaService.FirmarXmlAsync(xml, config.CertificadoBase64!, config.PasswordCertificado!);

        var recepcion = await _sriService.EnviarComprobanteAsync(xmlFirmado, config.Ambiente);
        if (!recepcion.Exitoso)
        {
            nd.Estado = EstadoSRI.RECHAZADA;
            nd.MotivoRechazo = string.Join(" | ", recepcion.Errores);
            nd.UpdatedAt = DateTime.UtcNow;
            await _ndRepo.UpdateAsync(nd);
            return new EmitirNotaDebitoResult(EstadoSRI.RECHAZADA, null, nd.MotivoRechazo, "ND rechazada por el SRI.");
        }

        nd.Estado = EstadoSRI.ENVIADA;
        nd.XmlFirmado = xmlFirmado;
        nd.UpdatedAt = DateTime.UtcNow;
        await _ndRepo.UpdateAsync(nd);

        await Task.Delay(2000, cancellationToken);
        var autorizacion = await _sriService.ConsultarAutorizacionAsync(nd.ClaveAcceso!, config.Ambiente);

        nd.Estado = autorizacion.Estado;
        nd.NumeroAutorizacion = autorizacion.NumeroAutorizacion;
        nd.FechaAutorizacion = autorizacion.FechaAutorizacion;
        nd.MotivoRechazo = autorizacion.MotivoRechazo;
        nd.UpdatedAt = DateTime.UtcNow;
        await _ndRepo.UpdateAsync(nd);

        return new EmitirNotaDebitoResult(autorizacion.Estado, autorizacion.NumeroAutorizacion, autorizacion.MotivoRechazo,
            autorizacion.Estado == EstadoSRI.AUTORIZADA ? $"ND autorizada. Nº: {autorizacion.NumeroAutorizacion}" : $"Estado: {autorizacion.Estado}");
    }
}
