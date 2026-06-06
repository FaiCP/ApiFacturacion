using Domain.Enums;

namespace Domain.Entities;

public class Producto : BaseEntity
{
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string? CodigoAuxiliar { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public TarifaIva TarifaIva { get; set; } = TarifaIva.Quince;
    public bool EsServicio { get; set; } = false;
    public bool Activo { get; set; } = true;

    public virtual ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
}
