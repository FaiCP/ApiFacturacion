namespace Application.DTOs.Emisor;

public class EmisorDto
{
    public long Id { get; set; }
    public string Ruc { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool ObligadoContabilidad { get; set; }
    public string Ambiente { get; set; } = string.Empty;
    public string SerieEstablecimiento { get; set; } = string.Empty;
    public string SeriePuntoEmision { get; set; } = string.Empty;
    public string? LogoBase64 { get; set; }
}
