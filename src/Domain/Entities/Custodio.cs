namespace Domain.Entities;

/// <summary>
/// Custodios de equipos
/// </summary>
public class Custodio : BaseEntity
{
    public string? Nombre { get; set; }
    public string? Cargo { get; set; }
    public string? Cedula { get; set; }
    public long? IdDepartamento { get; set; }

    // Navegación
    public virtual Departamento? Departamento { get; set; }
    public virtual ICollection<GestionActivo> GestionActivos { get; set; } = new List<GestionActivo>();
}
