using Domain.Enums;

namespace Application.DTOs.ConfiguracionSRI;

public class ConfiguracionSRICreateDto
{
    public string CertificadoBase64 { get; set; } = string.Empty;
    public string PasswordCertificado { get; set; } = string.Empty;
    public AmbienteSRI Ambiente { get; set; } = AmbienteSRI.Pruebas;
}
