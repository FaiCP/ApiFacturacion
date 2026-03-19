namespace Application.DTOs.Hardware;

public class HardwareDto
{
    public long Id { get; set; }
    public string IdEquipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public DateOnly? FechaAdquisicion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string CodigoCne { get; set; } = string.Empty;
    public bool? Borrado { get; set; }
    public string NombreDispositivo { get; set; } = string.Empty;
    public double? Valor { get; set; }
    public string NombreCustodio { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Ram { get; set; } = string.Empty;
    public string Rom { get; set; } = string.Empty;
    public string Procesador { get; set; } = string.Empty;
}
