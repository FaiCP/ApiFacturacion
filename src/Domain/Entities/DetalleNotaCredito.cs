using Domain.Enums;

namespace Domain.Entities;

public class DetalleNotaCredito : BaseEntity
{
    public long NotaCreditoId { get; set; }
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; } = 0;
    public decimal SubtotalSinImpuesto { get; set; }
    public TarifaIva TarifaIva { get; set; }
    public decimal ValorIva { get; set; }
    public decimal PrecioTotalSinImpuesto { get; set; }

    public virtual NotaCredito? NotaCredito { get; set; }
}
