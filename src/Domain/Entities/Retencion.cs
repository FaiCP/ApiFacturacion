using Domain.Enums;

namespace Domain.Entities;

public class Retencion : BaseEntity
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
    public string PeriodoFiscal { get; set; } = string.Empty;  // MM/yyyy

    public long FacturaId { get; set; }
    public long EmisorId { get; set; }
    public long SujetoRetenidoId { get; set; }

    public virtual Factura? Factura { get; set; }
    public virtual Emisor? Emisor { get; set; }
    public virtual Cliente? SujetoRetenido { get; set; }
    public virtual ICollection<DetalleRetencion> Detalles { get; set; } = new List<DetalleRetencion>();
}
