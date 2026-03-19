namespace Application.DTOs.Kits;

public class KitDto
{
    public long Id { get; set; }
    public string Insumo { get; set; } = string.Empty;
    public string Cantidad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
}
