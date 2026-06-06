using Application.DTOs.Clientes;
using Application.DTOs.Facturas;

namespace Application.DTOs.NotasCredito;

public class NotaCreditoDto
{
    public long Id { get; set; }
    public string? ClaveAcceso { get; set; }
    public string? NumeroAutorizacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? MotivoRechazo { get; set; }
    public DateTime FechaEmision { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Secuencial { get; set; } = string.Empty;
    public string NumeroCompleto => $"{Serie}-{Secuencial}";
    public string Motivo { get; set; } = string.Empty;
    public long FacturaId { get; set; }
    public string NumDocModificado { get; set; } = string.Empty;
    public decimal TotalSinImpuestos { get; set; }
    public decimal TotalIva { get; set; }
    public decimal ValorModificacion { get; set; }
    public ClienteDto? Cliente { get; set; }
    public List<DetalleFacturaDto> Detalles { get; set; } = new();
}
