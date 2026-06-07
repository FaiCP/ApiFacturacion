namespace Application.DTOs.ConfiguracionSRI;

public class ConfiguracionSRIDto
{
    public long Id { get; set; }
    public long EmisorId { get; set; }
    public string Ambiente { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime? FechaVencimientoCert { get; set; }
    public bool TieneCertificado { get; set; }
}
