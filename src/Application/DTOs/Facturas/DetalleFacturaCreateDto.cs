using Domain.Enums;

namespace Application.DTOs.Facturas;

public class DetalleFacturaCreateDto
{
    public long? ProductoId { get; set; }
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; } = 0;
    public TarifaIva TarifaIva { get; set; } = TarifaIva.Quince;
}
