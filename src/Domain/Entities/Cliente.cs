using Domain.Enums;

namespace Domain.Entities;

public class Cliente : BaseEntity
{
    public TipoIdentificacion TipoIdentificacion { get; set; }
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}
