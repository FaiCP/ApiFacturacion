using Domain.Enums;

namespace Domain.Entities;

public class Factura : BaseEntity
{
    public string? ClaveAcceso { get; set; }
    public string? NumeroAutorizacion { get; set; }
    public DateTime? FechaAutorizacion { get; set; }
    public EstadoSRI Estado { get; set; } = EstadoSRI.BORRADOR;
    public string? MotivoRechazo { get; set; }
    public string? XmlFirmado { get; set; }

    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public string Serie { get; set; } = string.Empty;       // ej. "001-001"
    public string Secuencial { get; set; } = string.Empty;  // ej. "000000001"

    public decimal TotalSinImpuestos { get; set; }
    public decimal TotalDescuento { get; set; }
    public decimal TotalIva { get; set; }
    public decimal ImporteTotal { get; set; }

    public long EmisorId { get; set; }
    public long ClienteId { get; set; }

    public virtual Emisor? Emisor { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public virtual ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
}
