namespace Domain.Entities;

/// <summary>
/// Kits de insumos
/// </summary>
public class Kit : BaseEntity
{
    public string Insumo { get; set; } = string.Empty;
    public string Cantidad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
}
