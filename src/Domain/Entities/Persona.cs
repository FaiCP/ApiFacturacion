namespace Domain.Entities;

/// <summary>
/// Personal de la organización
/// </summary>
public class Persona : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public DateTime? Fecha { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? TempPass { get; set; }
}
