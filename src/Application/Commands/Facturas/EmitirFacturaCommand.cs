using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Facturas;

public record EmitirFacturaCommand(long FacturaId) : IRequest<EmitirFacturaResult>;

public record EmitirFacturaResult(
    EstadoSRI Estado,
    string? NumeroAutorizacion,
    string? MotivoRechazo,
    string Mensaje);

public class EmitirFacturaCommandHandler : IRequestHandler<EmitirFacturaCommand, EmitirFacturaResult>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IEmisorRepository _emisorRepository;
    private readonly IConfiguracionSRIRepository _configRepo;
    private readonly IXmlFacturaService _xmlService;
    private readonly IComprobanteValidator _validator;
    private readonly IFirmaDigitalService _firmaService;
    private readonly ISRIService _sriService;

    public EmitirFacturaCommandHandler(
        IFacturaRepository facturaRepository,
        IEmisorRepository emisorRepository,
        IConfiguracionSRIRepository configRepo,
        IXmlFacturaService xmlService,
        IComprobanteValidator validator,
        IFirmaDigitalService firmaService,
        ISRIService sriService)
    {
        _facturaRepository = facturaRepository;
        _emisorRepository  = emisorRepository;
        _configRepo        = configRepo;
        _xmlService        = xmlService;
        _validator         = validator;
        _firmaService      = firmaService;
        _sriService        = sriService;
    }

    public async Task<EmitirFacturaResult> Handle(EmitirFacturaCommand request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.GetWithDetallesAsync(request.FacturaId)
            ?? throw new NotFoundException($"Factura {request.FacturaId} no encontrada.");

        if (factura.Estado == EstadoSRI.AUTORIZADA)
            return new EmitirFacturaResult(EstadoSRI.AUTORIZADA, factura.NumeroAutorizacion, null, "Factura ya está autorizada.");

        if (factura.Estado == EstadoSRI.ANULADA)
            throw new DomainException("No se puede emitir una factura anulada.");

        var config = await _configRepo.GetActivaAsync()
            ?? throw new NotFoundException("No hay configuración SRI activa. Suba el certificado digital primero.");

        var emisor = await _emisorRepository.GetByIdAsync(factura.EmisorId)
            ?? throw new NotFoundException("Emisor no encontrado.");
        factura.Emisor = emisor;

        if (config.FechaVencimientoCert.HasValue && config.FechaVencimientoCert.Value < DateTime.UtcNow)
            throw new DomainException($"Certificado digital venció el {config.FechaVencimientoCert.Value:dd/MM/yyyy}. Actualice el certificado en Configuración SRI.");

        if (emisor.Ambiente != config.Ambiente)
            throw new DomainException($"Conflicto de ambiente: emisor en {emisor.Ambiente} pero configuración SRI en {config.Ambiente}. Verifique la configuración.");

        // 1. Generar XML
        var xml = _xmlService.GenerarXml(factura);

        // 1b. Validar estructura y reglas SRI antes de firmar
        var validacion = _validator.Validar(xml);
        if (!validacion.EsValido)
        {
            factura.Estado = EstadoSRI.RECHAZADA;
            factura.MotivoRechazo = validacion.MensajeConsolidado();
            factura.UpdatedAt = DateTime.UtcNow;
            await _facturaRepository.UpdateAsync(factura);
            return new EmitirFacturaResult(EstadoSRI.RECHAZADA, null, factura.MotivoRechazo, "Comprobante inválido: no se envió al SRI.");
        }

        // 2. Firmar
        var xmlFirmado = await _firmaService.FirmarXmlAsync(
            xml, config.CertificadoBase64!, config.PasswordCertificado!);

        // 3. Enviar al SRI
        var recepcion = await _sriService.EnviarComprobanteAsync(xmlFirmado, config.Ambiente);

        if (!recepcion.Exitoso)
        {
            factura.Estado = EstadoSRI.RECHAZADA;
            factura.MotivoRechazo = string.Join(" | ", recepcion.Errores);
            factura.UpdatedAt = DateTime.UtcNow;
            await _facturaRepository.UpdateAsync(factura);
            return new EmitirFacturaResult(EstadoSRI.RECHAZADA, null, factura.MotivoRechazo, "Comprobante rechazado por el SRI.");
        }

        factura.Estado = EstadoSRI.ENVIADA;
        factura.XmlFirmado = xmlFirmado;
        factura.UpdatedAt = DateTime.UtcNow;
        await _facturaRepository.UpdateAsync(factura);

        // 4. Consultar autorización (pequeña espera para SRI)
        await Task.Delay(2000, cancellationToken);
        var autorizacion = await _sriService.ConsultarAutorizacionAsync(factura.ClaveAcceso!, config.Ambiente);

        factura.Estado = autorizacion.Estado;
        factura.NumeroAutorizacion = autorizacion.NumeroAutorizacion;
        factura.FechaAutorizacion = autorizacion.FechaAutorizacion;
        factura.MotivoRechazo = autorizacion.MotivoRechazo;
        factura.UpdatedAt = DateTime.UtcNow;
        await _facturaRepository.UpdateAsync(factura);

        var mensaje = autorizacion.Estado == EstadoSRI.AUTORIZADA
            ? $"Factura autorizada. Número: {autorizacion.NumeroAutorizacion}"
            : $"Comprobante en proceso. Estado: {autorizacion.Estado}";

        return new EmitirFacturaResult(autorizacion.Estado, autorizacion.NumeroAutorizacion, autorizacion.MotivoRechazo, mensaje);
    }
}
