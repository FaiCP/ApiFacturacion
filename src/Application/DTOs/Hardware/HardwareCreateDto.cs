namespace Application.DTOs.Hardware;

public class HardwareCreateDto
{
    public string Ubicacion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string NombreDispositivo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string CodigoCne { get; set; } = string.Empty;
    public string IdEquipo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Ram { get; set; } = string.Empty;
    public string Rom { get; set; } = string.Empty;
    public string Procesador { get; set; } = string.Empty;
    public double? Valor { get; set; }
}
