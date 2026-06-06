using Domain.Enums;

namespace Application.DTOs.Retenciones;

public class RetencionCreateDto
{
    public long FacturaId { get; set; }
    public long SujetoRetenidoId { get; set; }
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public string PeriodoFiscal { get; set; } = string.Empty;  // MM/yyyy
    public List<DetalleRetencionCreateDto> Detalles { get; set; } = new();
}

public class DetalleRetencionCreateDto
{
    public TipoImpuestoRetencion TipoImpuesto { get; set; }
    public string CodigoRetencion { get; set; } = string.Empty;
    public decimal BaseImponible { get; set; }
    public decimal PorcentajeRetener { get; set; }
}
