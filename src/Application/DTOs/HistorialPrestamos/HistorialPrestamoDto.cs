namespace Application.DTOs.HistorialPrestamos;

public class HistorialPrestamoDto
{
    public long Id { get; set; }
    public string IdEquipo { get; set; } = string.Empty;
    public long IdCustodio { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string NombreEmpleado { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CodigoCne { get; set; } = string.Empty;
}
