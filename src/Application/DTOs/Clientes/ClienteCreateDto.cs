using Domain.Enums;

namespace Application.DTOs.Clientes;

public class ClienteCreateDto
{
    public TipoIdentificacion TipoIdentificacion { get; set; }
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
