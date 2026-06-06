using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Infrastructure.Services;

public class RideService : IRideService
{
    private static readonly BaseColor ColorHeader  = new(41, 128, 185);
    private static readonly BaseColor ColorSubHeader = new(52, 73, 94);
    private static readonly BaseColor ColorRow2    = new(236, 240, 241);
    private static readonly Font FontTitle     = new(Font.HELVETICA, 14, Font.BOLD, new BaseColor(255, 255, 255));
    private static readonly Font FontSubtitle  = new(Font.HELVETICA, 9, Font.BOLD, new BaseColor(255, 255, 255));
    private static readonly Font FontNormal    = new(Font.HELVETICA, 8, Font.NORMAL);
    private static readonly Font FontBold      = new(Font.HELVETICA, 8, Font.BOLD);
    private static readonly Font FontSmall     = new(Font.HELVETICA, 7, Font.NORMAL, new BaseColor(128, 128, 128));

    public Task<byte[]> GenerarRideAsync(Factura factura)
    {
        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 30, 30, 30, 40);
        var writer = PdfWriter.GetInstance(document, stream);
        document.Open();

        // ── ENCABEZADO ──────────────────────────────────────────────────
        var tblHeader = new PdfPTable(2) { WidthPercentage = 100 };
        tblHeader.SetWidths(new float[] { 1.4f, 1f });

        // Celda izquierda: datos emisor
        var celdaEmisor = new PdfPCell { BackgroundColor = ColorHeader, Padding = 10, Border = 0 };
        celdaEmisor.AddElement(new Paragraph(factura.Emisor?.NombreComercial ?? factura.Emisor?.RazonSocial ?? "", FontTitle));
        celdaEmisor.AddElement(new Paragraph($"RUC: {factura.Emisor?.Ruc}", FontSubtitle));
        celdaEmisor.AddElement(new Paragraph(factura.Emisor?.Direccion ?? "", new Font(Font.HELVETICA, 8, Font.NORMAL, new BaseColor(255, 255, 255))));
        if (!string.IsNullOrEmpty(factura.Emisor?.Telefono))
            celdaEmisor.AddElement(new Paragraph($"Tel: {factura.Emisor.Telefono}", new Font(Font.HELVETICA, 8, Font.NORMAL, new BaseColor(255, 255, 255))));
        tblHeader.AddCell(celdaEmisor);

        // Celda derecha: identificación del documento
        var celdaDoc = new PdfPCell { BackgroundColor = ColorSubHeader, Padding = 10, Border = 0 };
        celdaDoc.AddElement(new Paragraph("FACTURA", FontTitle));
        celdaDoc.AddElement(new Paragraph($"Nº: {factura.Serie}-{factura.Secuencial}", FontSubtitle));
        celdaDoc.AddElement(new Paragraph($"Fecha: {factura.FechaEmision:dd/MM/yyyy}", new Font(Font.HELVETICA, 8, Font.NORMAL, new BaseColor(255, 255, 255))));
        celdaDoc.AddElement(new Paragraph($"Ambiente: {(factura.Emisor?.Ambiente == AmbienteSRI.Pruebas ? "PRUEBAS" : "PRODUCCIÓN")}", new Font(Font.HELVETICA, 7, Font.NORMAL, new BaseColor(255, 255, 255))));
        var estadoColor = factura.Estado == EstadoSRI.AUTORIZADA ? new BaseColor(0, 200, 83) : new BaseColor(255, 193, 7);
        celdaDoc.AddElement(new Paragraph($"Estado: {factura.Estado}", new Font(Font.HELVETICA, 8, Font.BOLD, estadoColor)));
        tblHeader.AddCell(celdaDoc);

        document.Add(tblHeader);
        document.Add(new Chunk("\n"));

        // ── CLAVE DE ACCESO ──────────────────────────────────────────────
        if (!string.IsNullOrEmpty(factura.ClaveAcceso))
        {
            var barcode = new Barcode128 { Code = factura.ClaveAcceso, BarHeight = 28, X = 1f };
            var tblBarcode = new PdfPTable(1) { WidthPercentage = 100 };
            var imgBarcode = barcode.CreateImageWithBarcode(writer.DirectContent, null, null);
            var celdaBarcode = new PdfPCell { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 4, Border = Rectangle.BOX };
            celdaBarcode.AddElement(new Paragraph("CLAVE DE ACCESO", FontSmall) { Alignment = Element.ALIGN_CENTER });
            celdaBarcode.Image = imgBarcode;
            celdaBarcode.AddElement(new Paragraph(factura.ClaveAcceso, FontSmall) { Alignment = Element.ALIGN_CENTER });
            tblBarcode.AddCell(celdaBarcode);
            document.Add(tblBarcode);
            document.Add(new Chunk("\n"));
        }

        // ── DATOS DEL CLIENTE ────────────────────────────────────────────
        var tblCliente = new PdfPTable(4) { WidthPercentage = 100 };
        tblCliente.SetWidths(new float[] { 1f, 2f, 1f, 2f });
        AgregarFilaCliente(tblCliente, "Razón Social:", factura.Cliente?.RazonSocial ?? "CONSUMIDOR FINAL", "Identificación:", factura.Cliente?.NumeroIdentificacion ?? "9999999999999");
        AgregarFilaCliente(tblCliente, "Dirección:", factura.Cliente?.Direccion ?? "", "Email:", factura.Cliente?.Email ?? "");
        document.Add(tblCliente);
        document.Add(new Chunk("\n"));

        // ── TABLA DE DETALLES ────────────────────────────────────────────
        var tblDetalles = new PdfPTable(6) { WidthPercentage = 100 };
        tblDetalles.SetWidths(new float[] { 1f, 4f, 1f, 1.2f, 1f, 1.2f });
        foreach (var h in new[] { "CÓDIGO", "DESCRIPCIÓN", "CANT.", "P. UNITARIO", "DESCUENTO", "SUBTOTAL" })
        {
            var cell = new PdfPCell(new Phrase(h, FontBold))
            { BackgroundColor = ColorHeader, Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER, Border = 0 };
            cell.Phrase.Font.Color = new BaseColor(255, 255, 255);
            tblDetalles.AddCell(cell);
        }

        bool altRow = false;
        foreach (var detalle in factura.Detalles)
        {
            var bg = altRow ? ColorRow2 : new BaseColor(255, 255, 255);
            AgregarCeldaDetalle(tblDetalles, detalle.CodigoPrincipal, bg, Element.ALIGN_LEFT);
            AgregarCeldaDetalle(tblDetalles, detalle.Descripcion, bg, Element.ALIGN_LEFT);
            AgregarCeldaDetalle(tblDetalles, detalle.Cantidad.ToString("N2"), bg, Element.ALIGN_CENTER);
            AgregarCeldaDetalle(tblDetalles, detalle.PrecioUnitario.ToString("N4"), bg, Element.ALIGN_RIGHT);
            AgregarCeldaDetalle(tblDetalles, detalle.Descuento.ToString("N2"), bg, Element.ALIGN_RIGHT);
            AgregarCeldaDetalle(tblDetalles, detalle.SubtotalSinImpuesto.ToString("N2"), bg, Element.ALIGN_RIGHT);
            altRow = !altRow;
        }

        document.Add(tblDetalles);
        document.Add(new Chunk("\n"));

        // ── TOTALES ──────────────────────────────────────────────────────
        var tblTotales = new PdfPTable(2) { WidthPercentage = 50, HorizontalAlignment = Element.ALIGN_RIGHT };
        tblTotales.SetWidths(new float[] { 2f, 1f });
        AgregarFilaTotales(tblTotales, "Subtotal 0%:", factura.Detalles.Where(d => d.TarifaIva == TarifaIva.Cero).Sum(d => d.SubtotalSinImpuesto).ToString("N2"));
        AgregarFilaTotales(tblTotales, "Subtotal 15%:", factura.Detalles.Where(d => d.TarifaIva == TarifaIva.Quince).Sum(d => d.SubtotalSinImpuesto).ToString("N2"));
        AgregarFilaTotales(tblTotales, "Subtotal No Objeto IVA:", factura.Detalles.Where(d => d.TarifaIva == TarifaIva.NoObjeto).Sum(d => d.SubtotalSinImpuesto).ToString("N2"));
        AgregarFilaTotales(tblTotales, "Subtotal Exento IVA:", factura.Detalles.Where(d => d.TarifaIva == TarifaIva.Exento).Sum(d => d.SubtotalSinImpuesto).ToString("N2"));
        AgregarFilaTotales(tblTotales, "Descuento:", $"-{factura.TotalDescuento:N2}");
        AgregarFilaTotales(tblTotales, "IVA 15%:", factura.TotalIva.ToString("N2"), bold: false);

        // Fila total destacada
        var celdaTotalLabel = new PdfPCell(new Phrase("VALOR TOTAL:", FontBold)) { BackgroundColor = ColorSubHeader, Padding = 5, Border = 0 };
        celdaTotalLabel.Phrase.Font.Color = new BaseColor(255, 255, 255);
        var celdaTotalValor = new PdfPCell(new Phrase($"$ {factura.ImporteTotal:N2}", new Font(Font.HELVETICA, 10, Font.BOLD, new BaseColor(255, 255, 255)))) { BackgroundColor = ColorSubHeader, Padding = 5, Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT };
        tblTotales.AddCell(celdaTotalLabel);
        tblTotales.AddCell(celdaTotalValor);
        document.Add(tblTotales);

        // ── PIE: AUTORIZACIÓN ────────────────────────────────────────────
        if (!string.IsNullOrEmpty(factura.NumeroAutorizacion))
        {
            document.Add(new Chunk("\n"));
            document.Add(new Paragraph($"Número de Autorización: {factura.NumeroAutorizacion}", FontSmall));
            document.Add(new Paragraph($"Fecha de Autorización: {factura.FechaAutorizacion:dd/MM/yyyy HH:mm:ss}", FontSmall));
        }

        document.Add(new Chunk("\n"));
        document.Add(new Paragraph("Documento generado electrónicamente. Autorizado por el SRI.", FontSmall) { Alignment = Element.ALIGN_CENTER });

        document.Close();
        return Task.FromResult(stream.ToArray());
    }

    private static void AgregarFilaCliente(PdfPTable table, string label1, string value1, string label2, string value2)
    {
        table.AddCell(new PdfPCell(new Phrase(label1, FontBold)) { Padding = 4, Border = Rectangle.BOTTOM_BORDER });
        table.AddCell(new PdfPCell(new Phrase(value1, FontNormal)) { Padding = 4, Border = Rectangle.BOTTOM_BORDER });
        table.AddCell(new PdfPCell(new Phrase(label2, FontBold)) { Padding = 4, Border = Rectangle.BOTTOM_BORDER });
        table.AddCell(new PdfPCell(new Phrase(value2, FontNormal)) { Padding = 4, Border = Rectangle.BOTTOM_BORDER });
    }

    private static void AgregarCeldaDetalle(PdfPTable table, string text, BaseColor bg, int alignment)
    {
        table.AddCell(new PdfPCell(new Phrase(text, FontNormal))
        { BackgroundColor = bg, Padding = 4, HorizontalAlignment = alignment, Border = 0 });
    }

    private static void AgregarFilaTotales(PdfPTable table, string label, string valor, bool bold = false)
    {
        var font = bold ? FontBold : FontNormal;
        table.AddCell(new PdfPCell(new Phrase(label, font)) { Padding = 4, Border = Rectangle.BOTTOM_BORDER });
        table.AddCell(new PdfPCell(new Phrase(valor, font)) { Padding = 4, Border = Rectangle.BOTTOM_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });
    }
}
