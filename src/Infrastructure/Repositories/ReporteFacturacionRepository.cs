using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReporteFacturacionRepository : IReporteFacturacionRepository
{
    private readonly ApplicationDbContext _db;
    public ReporteFacturacionRepository(ApplicationDbContext db) => _db = db;

    private static readonly EstadoSRI[] EstadosValidos = [EstadoSRI.AUTORIZADA, EstadoSRI.ENVIADA];

    public async Task<List<ReporteMesItem>> GetFacturasPorMesAsync(int anio)
    {
        var data = await _db.Facturas
            .AsNoTracking()
            .Where(f => f.FechaEmision.Year == anio && EstadosValidos.Contains(f.Estado))
            .GroupBy(f => new { f.FechaEmision.Year, f.FechaEmision.Month })
            .Select(g => new
            {
                g.Key.Year, g.Key.Month,
                Cantidad = g.Count(),
                Monto    = g.Sum(f => f.ImporteTotal),
                Iva      = g.Sum(f => f.TotalIva),
                SinImp   = g.Sum(f => f.TotalSinImpuestos)
            })
            .OrderBy(x => x.Month)
            .ToListAsync();

        return data.Select(d => new ReporteMesItem(d.Year, d.Month, d.Cantidad, d.Monto, d.Iva, d.SinImp)).ToList();
    }

    public async Task<List<ReporteMesIvaItem>> GetIvaPorMesAsync(int anio)
    {
        var facturas = await _db.Facturas
            .AsNoTracking()
            .Where(f => f.FechaEmision.Year == anio && EstadosValidos.Contains(f.Estado))
            .Include(f => f.Detalles)
            .ToListAsync();

        return facturas
            .GroupBy(f => new { f.FechaEmision.Year, f.FechaEmision.Month })
            .Select(g => new ReporteMesIvaItem(
                g.Key.Year, g.Key.Month,
                g.SelectMany(f => f.Detalles).Where(d => d.TarifaIva == TarifaIva.Cero).Sum(d => d.SubtotalSinImpuesto),
                g.SelectMany(f => f.Detalles).Where(d => d.TarifaIva == TarifaIva.Quince).Sum(d => d.SubtotalSinImpuesto),
                g.Sum(f => f.TotalIva),
                g.Sum(f => f.ImporteTotal)))
            .OrderBy(x => x.Mes)
            .ToList();
    }

    public async Task<List<ReporteEstadoItem>> GetFacturasPorEstadoAsync()
    {
        var data = await _db.Facturas
            .AsNoTracking()
            .Where(f => f.Borrado != true)
            .GroupBy(f => f.Estado)
            .Select(g => new { Estado = g.Key.ToString(), Cantidad = g.Count(), Total = g.Sum(f => f.ImporteTotal) })
            .ToListAsync();

        return data.Select(d => new ReporteEstadoItem(d.Estado, d.Cantidad, d.Total))
                   .OrderByDescending(x => x.TotalMonto)
                   .ToList();
    }

    public async Task<List<ReporteTopClienteItem>> GetTopClientesAsync(int top, int? anio)
    {
        var query = _db.Facturas
            .AsNoTracking()
            .Where(f => f.Estado == EstadoSRI.AUTORIZADA && f.Borrado != true)
            .Include(f => f.Cliente);

        IQueryable<Factura> filtrada = anio.HasValue ? query.Where(f => f.FechaEmision.Year == anio.Value) : query;

        return await filtrada
            .GroupBy(f => new { f.ClienteId, f.Cliente!.NumeroIdentificacion, f.Cliente.RazonSocial })
            .Select(g => new ReporteTopClienteItem(
                g.Key.NumeroIdentificacion, g.Key.RazonSocial,
                g.Count(), g.Sum(f => f.ImporteTotal)))
            .OrderByDescending(x => x.TotalFacturado)
            .Take(top)
            .ToListAsync();
    }

    public async Task<List<Factura>> GetFacturasParaAtsAsync(int anio, int mes) =>
        await _db.Facturas
            .AsNoTracking()
            .Where(f => f.FechaEmision.Year == anio && f.FechaEmision.Month == mes && f.Estado == EstadoSRI.AUTORIZADA)
            .Include(f => f.Cliente)
            .Include(f => f.Emisor)
            .Include(f => f.Detalles)
            .ToListAsync();

    public async Task<List<Retencion>> GetRetencionesParaAtsAsync(int anio, int mes) =>
        await _db.Retenciones
            .AsNoTracking()
            .Where(r => r.FechaEmision.Year == anio && r.FechaEmision.Month == mes && r.Estado == EstadoSRI.AUTORIZADA)
            .Include(r => r.SujetoRetenido)
            .Include(r => r.Detalles)
            .ToListAsync();

    public async Task<List<Factura>> GetFacturasConXmlAsync(DateTime desde, DateTime hasta) =>
        await _db.Facturas
            .AsNoTracking()
            .Where(f => f.FechaEmision >= desde && f.FechaEmision <= hasta
                     && f.Estado == EstadoSRI.AUTORIZADA && !string.IsNullOrEmpty(f.XmlFirmado))
            .Select(f => new Factura { Id = f.Id, ClaveAcceso = f.ClaveAcceso, XmlFirmado = f.XmlFirmado, Serie = f.Serie, Secuencial = f.Secuencial })
            .ToListAsync();
}
