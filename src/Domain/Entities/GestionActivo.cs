namespace Domain.Entities;

/// <summary>
/// Gestión de activos asignados (gestion_activos)
/// </summary>
public class GestionActivo : BaseEntity
{
    public string? IdEquipo { get; set; }
    public long? IdCustodio { get; set; }
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaDevolucion { get; set; }

    // Navegación
    public virtual Custodio? Custodio { get; set; }
    public virtual Hardware? Hardware { get; set; }
}
