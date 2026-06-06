using Application.DTOs.Facturas;

namespace Application.DTOs.NotasCredito;

public class NotaCreditoCreateDto
{
    public long FacturaId { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public List<DetalleFacturaCreateDto> Detalles { get; set; } = new();
}
