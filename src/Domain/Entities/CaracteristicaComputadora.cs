namespace Domain.Entities;

/// <summary>
/// Características de computadora (caracteristicas_computadora)
/// </summary>
public class CaracteristicaComputadora : BaseEntity
{
    public string? IdEquipo { get; set; }
    public string? Ram { get; set; }
    public string? Rom { get; set; }
    public string? Procesador { get; set; }

    // Clave foránea
    public long? HardwareId { get; set; }
    public virtual Hardware? Hardware { get; set; }
}
