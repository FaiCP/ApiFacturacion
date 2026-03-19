using Application.DTOs.Documentos;
using Application.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Services;

public class ExcelService : IExcelService
{
    static ExcelService()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public byte[] GenerarActaHardware(List<HardwareActaItemDto> items)
    {
        using var stream = new MemoryStream();
        using var package = new ExcelPackage(stream);
        var ws = package.Workbook.Worksheets.Add("Informe de Inventarios");

        ws.Cells[1, 1].Value = "Informe de Inventarios";
        ws.Cells[1, 1].Style.Font.Size = 16;
        ws.Cells[1, 1].Style.Font.Bold = true;
        ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells[1, 1, 1, 12].Merge = true;

        string[] headers = { "Nº", "ACTIVO", "CUSTODIO", "DESCRIPCION", "MARCA", "MODELO", "SERIE", "RAM", "DISCO DURO", "PROCESADOR", "VALOR", "ESTADO" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cells[3, i + 1].Value = headers[i];
            ws.Cells[3, i + 1].Style.Font.Bold = true;
            ws.Cells[3, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        int row = 4;
        int cnt = 1;
        foreach (var item in items)
        {
            ws.Cells[row, 1].Value = cnt++;
            ws.Cells[row, 2].Value = item.IdEquipo;
            ws.Cells[row, 3].Value = item.NombreCustodio;
            ws.Cells[row, 4].Value = item.NombreDispositivo;
            ws.Cells[row, 5].Value = item.Marca;
            ws.Cells[row, 6].Value = item.Modelo;
            ws.Cells[row, 7].Value = item.CodigoCne;
            ws.Cells[row, 8].Value = item.Ram;
            ws.Cells[row, 9].Value = item.Rom;
            ws.Cells[row, 10].Value = item.Procesador;
            ws.Cells[row, 11].Value = item.Valor;
            ws.Cells[row, 12].Value = item.Estado;
            row++;
        }

        for (int i = 1; i <= headers.Length; i++)
            ws.Column(i).AutoFit();

        package.Save();
        return stream.ToArray();
    }

    public byte[] GenerarReportePersonal(List<ReportePersonalItemDto> items)
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Informe");

        string[] headers = { "Fecha", "Entrega", "Recibe", "Equipos", "Sistemas", "Observación" };
        for (int i = 0; i < headers.Length; i++)
            ws.Cells[1, i + 1].Value = headers[i];

        using (var range = ws.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        int fila = 2;
        foreach (var item in items)
        {
            ws.Cells[fila, 1].Value = item.Fecha?.ToString("yyyy-MM-dd") ?? "";
            ws.Cells[fila, 2].Value = item.Entrega;
            ws.Cells[fila, 3].Value = item.Recibe;
            ws.Cells[fila, 4].Value = item.EquiposE;
            ws.Cells[fila, 5].Value = item.EquiposP;
            ws.Cells[fila, 6].Value = item.Observacion;
            fila++;
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        return package.GetAsByteArray();
    }
}
