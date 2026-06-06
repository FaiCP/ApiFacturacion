using Domain.Enums;

namespace Application.DTOs.Emisor;

public class EmisorCreateDto
{
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool ObligadoContabilidad { get; set; }
    public AmbienteSRI Ambiente { get; set; } = AmbienteSRI.Pruebas;
    public string SerieEstablecimiento { get; set; } = "001";
    public string SeriePuntoEmision { get; set; } = "001";
    public string? LogoBase64 { get; set; }
}
