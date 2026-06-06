namespace Application.DTOs.Facturas;

public class FacturaResumenDto
{
    public long Id { get; set; }
    public string? ClaveAcceso { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string NumeroCompleto { get; set; } = string.Empty;
    public string RazonSocialCliente { get; set; } = string.Empty;
    public string IdentificacionCliente { get; set; } = string.Empty;
    public decimal ImporteTotal { get; set; }
}
