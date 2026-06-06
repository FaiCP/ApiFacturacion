using Domain.Entities;

namespace Domain.Interfaces;

public interface IReporteFacturacionRepository
{
    Task<List<ReporteMesItem>> GetFacturasPorMesAsync(int anio);
    Task<List<ReporteMesIvaItem>> GetIvaPorMesAsync(int anio);
    Task<List<ReporteEstadoItem>> GetFacturasPorEstadoAsync();
    Task<List<ReporteTopClienteItem>> GetTopClientesAsync(int top, int? anio);
    Task<List<Factura>> GetFacturasParaAtsAsync(int anio, int mes);
    Task<List<Retencion>> GetRetencionesParaAtsAsync(int anio, int mes);
    Task<List<Factura>> GetFacturasConXmlAsync(DateTime desde, DateTime hasta);
}

public record ReporteMesItem(int Anio, int Mes, int Cantidad, decimal TotalMonto, decimal TotalIva, decimal TotalSinImpuestos);
public record ReporteMesIvaItem(int Anio, int Mes, decimal Base0, decimal Base15, decimal TotalIva, decimal ImporteTotal);
public record ReporteEstadoItem(string Estado, int Cantidad, decimal TotalMonto);
public record ReporteTopClienteItem(string Identificacion, string RazonSocial, int TotalFacturas, decimal TotalFacturado);
