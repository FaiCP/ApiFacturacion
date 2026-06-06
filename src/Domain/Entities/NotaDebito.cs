using Domain.Enums;

namespace Domain.Entities;

public class NotaDebito : BaseEntity
{
    public string? ClaveAcceso { get; set; }
    public string? NumeroAutorizacion { get; set; }
    public DateTime? FechaAutorizacion { get; set; }
    public EstadoSRI Estado { get; set; } = EstadoSRI.BORRADOR;
    public string? MotivoRechazo { get; set; }
    public string? XmlFirmado { get; set; }

    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public string Serie { get; set; } = string.Empty;
    public string Secuencial { get; set; } = string.Empty;

    public long FacturaId { get; set; }
    public string NumDocModificado { get; set; } = string.Empty;
    public DateTime FechaEmisionDocSustento { get; set; }

    public decimal TotalSinImpuestos { get; set; }
    public decimal TotalIva { get; set; }
    public decimal ValorTotal { get; set; }

    public long EmisorId { get; set; }
    public long ClienteId { get; set; }

    public virtual Factura? Factura { get; set; }
    public virtual Emisor? Emisor { get; set; }
    public virtual Cliente? Cliente { get; set; }
    public virtual ICollection<MotivoNotaDebito> Motivos { get; set; } = new List<MotivoNotaDebito>();
}
