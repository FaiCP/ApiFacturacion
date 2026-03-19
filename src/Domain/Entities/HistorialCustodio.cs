namespace Domain.Entities;

/// <summary>
/// Historial de cambios de custodios
/// </summary>
public class HistorialCustodio : BaseEntity
{
    public DateTime? TimestampEvento { get; set; }
    public string Custodio { get; set; } = string.Empty;
    public long? IdDepartamento { get; set; }
}
