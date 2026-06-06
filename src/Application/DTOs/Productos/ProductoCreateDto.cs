using Domain.Enums;

namespace Application.DTOs.Productos;

public class ProductoCreateDto
{
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string? CodigoAuxiliar { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public TarifaIva TarifaIva { get; set; } = TarifaIva.Quince;
    public bool EsServicio { get; set; } = false;
}
