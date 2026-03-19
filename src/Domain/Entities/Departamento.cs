namespace Domain.Entities;

/// <summary>
/// Departamentos de la organización
/// </summary>
public class Departamento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;

    // Navegación
    public virtual ICollection<Custodio> Custodios { get; set; } = new List<Custodio>();
}
