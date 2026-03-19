namespace Application.DTOs.Suministros;

public class SuministroDto
{
    public long Id { get; set; }
    public string IdEquipo { get; set; } = string.Empty;
    public string TipoSuministro { get; set; } = string.Empty;
    public string IdEquipoAsignado { get; set; } = string.Empty;
    public DateTime? FechaRetiro { get; set; }
}
