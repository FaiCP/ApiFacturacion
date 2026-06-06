using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.NotasCredito;

public record EmitirNotaCreditoCommand(long NotaCreditoId) : IRequest<EmitirNotaCreditoResult>;
public record EmitirNotaCreditoResult(EstadoSRI Estado, string? NumeroAutorizacion, string? MotivoRechazo, string Mensaje);

public class EmitirNotaCreditoCommandHandler : IRequestHandler<EmitirNotaCreditoCommand, EmitirNotaCreditoResult>
{
    private readonly INotaCreditoRepository _ncRepo;
    private readonly IEmisorRepository _emisorRepo;
    private readonly IConfiguracionSRIRepository _configRepo;
    private readonly IXmlNotaCreditoService _xmlService;
    private readonly IFirmaDigitalService _firmaService;
    private readonly ISRIService _sriService;

    public EmitirNotaCreditoCommandHandler(
        INotaCreditoRepository ncRepo, IEmisorRepository emisorRepo,
        IConfiguracionSRIRepository configRepo, IXmlNotaCreditoService xmlService,
        IFirmaDigitalService firmaService, ISRIService sriService)
    {
        _ncRepo = ncRepo; _emisorRepo = emisorRepo; _configRepo = configRepo;
        _xmlService = xmlService; _firmaService = firmaService; _sriService = sriService;
    }

    public async Task<EmitirNotaCreditoResult> Handle(EmitirNotaCreditoCommand request, CancellationToken cancellationToken)
    {
        var nc = await _ncRepo.GetWithDetallesAsync(request.NotaCreditoId)
            ?? throw new NotFoundException($"Nota de Crédito {request.NotaCreditoId} no encontrada.");

        if (nc.Estado == EstadoSRI.ANULADA) throw new DomainException("No se puede emitir una NC anulada.");
        if (nc.Estado == EstadoSRI.AUTORIZADA) return new EmitirNotaCreditoResult(EstadoSRI.AUTORIZADA, nc.NumeroAutorizacion, null, "NC ya autorizada.");

        var config = await _configRepo.GetActivaAsync()
            ?? throw new NotFoundException("No hay configuración SRI activa.");
        var emisor = await _emisorRepo.GetByIdAsync(nc.EmisorId) ?? throw new NotFoundException("Emisor no encontrado.");
        nc.Emisor = emisor;

        var xml = _xmlService.GenerarXml(nc);
        var xmlFirmado = await _firmaService.FirmarXmlAsync(xml, config.CertificadoBase64!, config.PasswordCertificado!);

        var recepcion = await _sriService.EnviarComprobanteAsync(xmlFirmado, config.Ambiente);
        if (!recepcion.Exitoso)
        {
            nc.Estado = EstadoSRI.RECHAZADA;
            nc.MotivoRechazo = string.Join(" | ", recepcion.Errores);
            nc.UpdatedAt = DateTime.UtcNow;
            await _ncRepo.UpdateAsync(nc);
            return new EmitirNotaCreditoResult(EstadoSRI.RECHAZADA, null, nc.MotivoRechazo, "NC rechazada por el SRI.");
        }

        nc.Estado = EstadoSRI.ENVIADA;
        nc.XmlFirmado = xmlFirmado;
        nc.UpdatedAt = DateTime.UtcNow;
        await _ncRepo.UpdateAsync(nc);

        await Task.Delay(2000, cancellationToken);
        var autorizacion = await _sriService.ConsultarAutorizacionAsync(nc.ClaveAcceso!, config.Ambiente);

        nc.Estado = autorizacion.Estado;
        nc.NumeroAutorizacion = autorizacion.NumeroAutorizacion;
        nc.FechaAutorizacion = autorizacion.FechaAutorizacion;
        nc.MotivoRechazo = autorizacion.MotivoRechazo;
        nc.UpdatedAt = DateTime.UtcNow;
        await _ncRepo.UpdateAsync(nc);

        return new EmitirNotaCreditoResult(autorizacion.Estado, autorizacion.NumeroAutorizacion, autorizacion.MotivoRechazo,
            autorizacion.Estado == EstadoSRI.AUTORIZADA ? $"NC autorizada. Nº: {autorizacion.NumeroAutorizacion}" : $"Estado: {autorizacion.Estado}");
    }
}
