namespace Application.DTOs.GestionActivos;

public class GestionActivoDto
{
    public long Id { get; set; }
    public string IdEquipo { get; set; } = string.Empty;
    public long? IdCustodio { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public long? IdDepartamento { get; set; }
    public string NombreEmpleado { get; set; } = string.Empty;
    public bool? Borrado { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public DateTime? FechaAdquisicion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string CodigoCne { get; set; } = string.Empty;
    public string NombreDispositivo { get; set; } = string.Empty;
}
