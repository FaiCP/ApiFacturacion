namespace Domain.Entities;

/// <summary>
/// Suministros remanufacturados
/// </summary>
public class Suministro : BaseEntity
{
    public string? IdEquipo { get; set; }
    public string? TipoSuministro { get; set; }
    public DateTime? FechaRetiro { get; set; }
    public string? IdEquipoAsignado { get; set; }

    // Clave foránea
    public long? HardwareId { get; set; }
    public virtual Hardware? Hardware { get; set; }
}
