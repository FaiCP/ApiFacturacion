using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Infrastructure.Services;

/// <summary>
/// Genera el ATS (Anexo Transaccional Simplificado) en formato Excel para declaración SRI mensual.
/// </summary>
public class AtsExcelService : IAtsExcelService
{
    static AtsExcelService()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public byte[] GenerarAts(List<Factura> facturas, List<Retencion> retenciones, int anio, int mes)
    {
        using var ms = new MemoryStream();
        using var pkg = new ExcelPackage(ms);

        var emisor = facturas.FirstOrDefault()?.Emisor;
        var periodo = new DateTime(anio, mes, 1).ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-EC")).ToUpper();

        // ── Hoja 1: Resumen ──────────────────────────────────────────────
        var wsResumen = pkg.Workbook.Worksheets.Add("RESUMEN");
        wsResumen.Cells[1, 1].Value = "ANEXO TRANSACCIONAL SIMPLIFICADO";
        wsResumen.Cells[1, 1].Style.Font.Bold = true;
        wsResumen.Cells[1, 1].Style.Font.Size = 14;
        wsResumen.Cells[1, 1, 1, 4].Merge = true;
        wsResumen.Cells[2, 1].Value = $"Período: {periodo}";
        wsResumen.Cells[3, 1].Value = $"RUC Emisor: {emisor?.Ruc ?? "-"}";
        wsResumen.Cells[4, 1].Value = $"Razón Social: {emisor?.RazonSocial ?? "-"}";
        wsResumen.Cells[6, 1].Value = "RESUMEN DE VENTAS";
        wsResumen.Cells[6, 1].Style.Font.Bold = true;

        var facturasAut = facturas.Where(f => f.Estado == EstadoSRI.AUTORIZADA).ToList();
        wsResumen.Cells[7, 1].Value = "Total facturas autorizadas:";
        wsResumen.Cells[7, 2].Value = facturasAut.Count;
        wsResumen.Cells[8, 1].Value = "Total base imponible 0%:";
        wsResumen.Cells[8, 2].Value = facturasAut.SelectMany(f => f.Detalles).Where(d => d.TarifaIva == TarifaIva.Cero).Sum(d => d.SubtotalSinImpuesto);
        wsResumen.Cells[8, 2].Style.Numberformat.Format = "#,##0.00";
        wsResumen.Cells[9, 1].Value = "Total base imponible 15%:";
        wsResumen.Cells[9, 2].Value = facturasAut.SelectMany(f => f.Detalles).Where(d => d.TarifaIva == TarifaIva.Quince).Sum(d => d.SubtotalSinImpuesto);
        wsResumen.Cells[9, 2].Style.Numberformat.Format = "#,##0.00";
        wsResumen.Cells[10, 1].Value = "Total IVA 15%:";
        wsResumen.Cells[10, 2].Value = facturasAut.Sum(f => f.TotalIva);
        wsResumen.Cells[10, 2].Style.Numberformat.Format = "#,##0.00";
        wsResumen.Cells[11, 1].Value = "Total importe:";
        wsResumen.Cells[11, 2].Value = facturasAut.Sum(f => f.ImporteTotal);
        wsResumen.Cells[11, 2].Style.Numberformat.Format = "#,##0.00";

        wsResumen.Cells[13, 1].Value = "Total retenciones IR:";
        wsResumen.Cells[13, 2].Value = retenciones.SelectMany(r => r.Detalles).Where(d => d.TipoImpuesto == TipoImpuestoRetencion.Renta).Sum(d => d.ValorRetenido);
        wsResumen.Cells[13, 2].Style.Numberformat.Format = "#,##0.00";
        wsResumen.Cells[14, 1].Value = "Total retenciones IVA:";
        wsResumen.Cells[14, 2].Value = retenciones.SelectMany(r => r.Detalles).Where(d => d.TipoImpuesto == TipoImpuestoRetencion.IVA).Sum(d => d.ValorRetenido);
        wsResumen.Cells[14, 2].Style.Numberformat.Format = "#,##0.00";
        wsResumen.Column(1).Width = 30;
        wsResumen.Column(2).Width = 18;

        // ── Hoja 2: Facturas emitidas ────────────────────────────────────
        var wsFacturas = pkg.Workbook.Worksheets.Add("VENTAS");
        string[] hdrsF = { "Nº", "TIPO DOC", "SERIE", "SECUENCIAL", "CLAVE ACCESO", "FECHA EMISIÓN",
                           "ID CLIENTE", "RAZÓN SOCIAL", "BASE 0%", "BASE 15%", "IVA 15%", "IMPORTE TOTAL", "ESTADO" };
        EstablecerEncabezados(wsFacturas, hdrsF, 1);

        int row = 2;
        foreach (var f in facturas.OrderBy(f => f.FechaEmision))
        {
            wsFacturas.Cells[row, 1].Value = row - 1;
            wsFacturas.Cells[row, 2].Value = "01";
            wsFacturas.Cells[row, 3].Value = f.Serie;
            wsFacturas.Cells[row, 4].Value = f.Secuencial;
            wsFacturas.Cells[row, 5].Value = f.ClaveAcceso;
            wsFacturas.Cells[row, 6].Value = f.FechaEmision.ToString("dd/MM/yyyy");
            wsFacturas.Cells[row, 7].Value = f.Cliente?.NumeroIdentificacion;
            wsFacturas.Cells[row, 8].Value = f.Cliente?.RazonSocial;
            wsFacturas.Cells[row, 9].Value = f.Detalles.Where(d => d.TarifaIva == TarifaIva.Cero).Sum(d => d.SubtotalSinImpuesto);
            wsFacturas.Cells[row, 10].Value = f.Detalles.Where(d => d.TarifaIva == TarifaIva.Quince).Sum(d => d.SubtotalSinImpuesto);
            wsFacturas.Cells[row, 11].Value = f.TotalIva;
            wsFacturas.Cells[row, 12].Value = f.ImporteTotal;
            wsFacturas.Cells[row, 13].Value = f.Estado.ToString();
            foreach (var col in new[] { 9, 10, 11, 12 })
                wsFacturas.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
            row++;
        }
        AutoFitColumns(wsFacturas, hdrsF.Length);

        // ── Hoja 3: Retenciones emitidas ─────────────────────────────────
        var wsRet = pkg.Workbook.Worksheets.Add("RETENCIONES");
        string[] hdrsR = { "Nº", "SERIE", "SECUENCIAL", "FECHA", "ID RETENIDO", "RAZÓN SOCIAL",
                           "TIPO IMP", "CÓD RETENCIÓN", "BASE IMPONIBLE", "% RETENCIÓN", "VALOR RETENIDO", "NUM DOC SUSTENTO" };
        EstablecerEncabezados(wsRet, hdrsR, 1);

        row = 2;
        foreach (var r in retenciones.OrderBy(r => r.FechaEmision))
        {
            foreach (var d in r.Detalles)
            {
                wsRet.Cells[row, 1].Value = row - 1;
                wsRet.Cells[row, 2].Value = r.Serie;
                wsRet.Cells[row, 3].Value = r.Secuencial;
                wsRet.Cells[row, 4].Value = r.FechaEmision.ToString("dd/MM/yyyy");
                wsRet.Cells[row, 5].Value = r.SujetoRetenido?.NumeroIdentificacion;
                wsRet.Cells[row, 6].Value = r.SujetoRetenido?.RazonSocial;
                wsRet.Cells[row, 7].Value = d.TipoImpuesto == TipoImpuestoRetencion.Renta ? "IR" : "IVA";
                wsRet.Cells[row, 8].Value = d.CodigoRetencion;
                wsRet.Cells[row, 9].Value = d.BaseImponible;
                wsRet.Cells[row, 10].Value = d.PorcentajeRetener;
                wsRet.Cells[row, 11].Value = d.ValorRetenido;
                wsRet.Cells[row, 12].Value = d.NumDocSustento;
                foreach (var col in new[] { 9, 10, 11 })
                    wsRet.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                row++;
            }
        }
        AutoFitColumns(wsRet, hdrsR.Length);

        pkg.Save();
        return ms.ToArray();
    }

    private static void EstablecerEncabezados(ExcelWorksheet ws, string[] headers, int fila)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cells[fila, i + 1].Value = headers[i];
            ws.Cells[fila, i + 1].Style.Font.Bold = true;
            ws.Cells[fila, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[fila, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(41, 128, 185));
            ws.Cells[fila, i + 1].Style.Font.Color.SetColor(Color.White);
            ws.Cells[fila, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }

    private static void AutoFitColumns(ExcelWorksheet ws, int count)
    {
        for (int i = 1; i <= count; i++)
            ws.Column(i).AutoFit();
    }
}
