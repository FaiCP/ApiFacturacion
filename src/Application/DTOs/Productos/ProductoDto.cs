namespace Application.DTOs.Productos;

public class ProductoDto
{
    public long Id { get; set; }
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string? CodigoAuxiliar { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal PrecioUnitario { get; set; }
    public string TarifaIva { get; set; } = string.Empty;
    public decimal PorcentajeIva { get; set; }
    public bool EsServicio { get; set; }
    public bool Activo { get; set; }
}
