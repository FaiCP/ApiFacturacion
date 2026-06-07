using Domain.Enums;

namespace Domain.Entities;

public class ConfiguracionSRI : BaseEntity
{
    public long EmisorId { get; set; }
    public virtual Emisor Emisor { get; set; } = null!;
    public string? CertificadoBase64 { get; set; }
    public string? PasswordCertificado { get; set; }  // almacenar cifrado
    public AmbienteSRI Ambiente { get; set; } = AmbienteSRI.Pruebas;
    public bool Activo { get; set; } = true;
    public DateTime? FechaVencimientoCert { get; set; }
}
