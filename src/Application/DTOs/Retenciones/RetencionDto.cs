using Application.DTOs.Clientes;
using Domain.Enums;

namespace Application.DTOs.Retenciones;

public class RetencionDto
{
    public long Id { get; set; }
    public string? ClaveAcceso { get; set; }
    public string? NumeroAutorizacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? MotivoRechazo { get; set; }
    public DateTime FechaEmision { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Secuencial { get; set; } = string.Empty;
    public string NumeroCompleto => $"{Serie}-{Secuencial}";
    public string PeriodoFiscal { get; set; } = string.Empty;
    public long FacturaId { get; set; }
    public ClienteDto? SujetoRetenido { get; set; }
    public List<DetalleRetencionDto> Detalles { get; set; } = new();
    public decimal TotalRetenido => Detalles.Sum(d => d.ValorRetenido);
}

public class DetalleRetencionDto
{
    public string TipoImpuesto { get; set; } = string.Empty;
    public string CodigoRetencion { get; set; } = string.Empty;
    public decimal BaseImponible { get; set; }
    public decimal PorcentajeRetener { get; set; }
    public decimal ValorRetenido { get; set; }
    public string NumDocSustento { get; set; } = string.Empty;
}
