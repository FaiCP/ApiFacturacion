using Domain.Enums;

namespace Domain.Entities;

public class DetalleRetencion : BaseEntity
{
    public long RetencionId { get; set; }
    public TipoImpuestoRetencion TipoImpuesto { get; set; }
    public string CodigoRetencion { get; set; } = string.Empty;  // ej. "302", "303", "725"
    public decimal BaseImponible { get; set; }
    public decimal PorcentajeRetener { get; set; }
    public decimal ValorRetenido { get; set; }
    public string CodDocSustento { get; set; } = "01";   // "01" = factura
    public string NumDocSustento { get; set; } = string.Empty;
    public DateTime FechaEmisionDocSustento { get; set; }

    public virtual Retencion? Retencion { get; set; }
}
