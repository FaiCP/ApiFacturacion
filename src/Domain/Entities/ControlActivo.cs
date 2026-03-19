namespace Domain.Entities;

/// <summary>
/// Control de activos (auditoría)
/// </summary>
public class ControlActivo : BaseEntity
{
    public string? IdEquipo { get; set; }
    public DateTime? FechaAuditoria { get; set; }
    public string? DetallesAuditoria { get; set; }
    public string? Custodio { get; set; }

    // Clave foránea
    public long? HardwareId { get; set; }
    public virtual Hardware? Hardware { get; set; }
}
