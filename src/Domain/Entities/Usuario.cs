namespace Domain.Entities;

/// <summary>
/// Usuarios del sistema
/// </summary>
public class Usuario : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Se almacenará hasheada
    public string Cargo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
