namespace Application.DTOs.Facturas;

public class FacturaCreateDto
{
    public long ClienteId { get; set; }
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public List<DetalleFacturaCreateDto> Detalles { get; set; } = new();
}
