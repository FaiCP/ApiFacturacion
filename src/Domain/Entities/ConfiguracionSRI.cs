using Domain.Enums;

namespace Domain.Entities;

public class ConfiguracionSRI : BaseEntity
{
    public string? CertificadoBase64 { get; set; }
    public string? PasswordCertificado { get; set; }  // almacenar cifrado
    public AmbienteSRI Ambiente { get; set; } = AmbienteSRI.Pruebas;
    public bool Activo { get; set; } = true;
    public DateTime? FechaVencimientoCert { get; set; }
}
