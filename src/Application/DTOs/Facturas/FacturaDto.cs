using Application.DTOs.Clientes;

namespace Application.DTOs.Facturas;

public class FacturaDto
{
    public long Id { get; set; }
    public string? ClaveAcceso { get; set; }
    public string? NumeroAutorizacion { get; set; }
    public DateTime? FechaAutorizacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? MotivoRechazo { get; set; }
    public DateTime FechaEmision { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Secuencial { get; set; } = string.Empty;
    public string NumeroCompleto => $"{Serie}-{Secuencial}";

    public decimal TotalSinImpuestos { get; set; }
    public decimal TotalDescuento { get; set; }
    public decimal TotalIva { get; set; }
    public decimal ImporteTotal { get; set; }

    public ClienteDto? Cliente { get; set; }
    public List<DetalleFacturaDto> Detalles { get; set; } = new();
}
