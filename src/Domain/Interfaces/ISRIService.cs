using Domain.Enums;

namespace Domain.Interfaces;

public interface ISRIService
{
    Task<SRIRecepcionResult> EnviarComprobanteAsync(string xmlFirmado, AmbienteSRI ambiente);
    Task<SRIAutorizacionResult> ConsultarAutorizacionAsync(string claveAcceso, AmbienteSRI ambiente);
}

public record SRIRecepcionResult(bool Exitoso, string Mensaje, IReadOnlyList<string> Errores);

public record SRIAutorizacionResult(
    EstadoSRI Estado,
    string? NumeroAutorizacion,
    DateTime? FechaAutorizacion,
    string? MotivoRechazo
);
