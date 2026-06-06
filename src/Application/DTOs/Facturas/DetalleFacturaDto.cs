namespace Application.DTOs.Facturas;

public class DetalleFacturaDto
{
    public long Id { get; set; }
    public long? ProductoId { get; set; }
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal SubtotalSinImpuesto { get; set; }
    public string TarifaIva { get; set; } = string.Empty;
    public decimal ValorIva { get; set; }
    public decimal PrecioTotalSinImpuesto { get; set; }
}
