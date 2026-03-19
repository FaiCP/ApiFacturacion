namespace Domain.Entities;

/// <summary>
/// Entidad base con propiedades comunes para todas las entidades
/// </summary>
public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool? Borrado { get; set; } = false;
}
