using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Infrastructure.Services;

/// <summary>
/// Manejador de eventos de página PDF — encabezado con logo institucional y pie de página
/// </summary>
public class EncabezadoPageEvent : PdfPageEventHelper
{
    private static byte[]? _logoBytes;
    private static byte[]? _footerBytes;

    public override void OnStartPage(PdfWriter writer, Document document)
    {
        base.OnStartPage(writer, document);

        float tableWidth = document.Right - document.Left;
        PdfPTable headerTable = new PdfPTable(1);
        headerTable.TotalWidth = tableWidth;

        try
        {
            _logoBytes ??= DownloadImage("https://i.postimg.cc/76n2VdB1/Captura1.png");
            var logo = Image.GetInstance(_logoBytes);
            logo.ScaleAbsolute(tableWidth, 90);
            headerTable.AddCell(new PdfPCell(logo)
            {
                Border = PdfPCell.NO_BORDER,
                Padding = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            });
        }
        catch
        {
            headerTable.AddCell(new PdfPCell { Border = PdfPCell.NO_BORDER, FixedHeight = 90 });
        }

        headerTable.WriteSelectedRows(0, -1, document.Left, document.PageSize.Height - 2, writer.DirectContent);
    }

    public override void OnEndPage(PdfWriter writer, Document document)
    {
        base.OnEndPage(writer, document);

        float tableWidth = document.Right - document.Left;
        PdfPTable footerTable = new PdfPTable(2);
        footerTable.TotalWidth = tableWidth;
        footerTable.SetWidths(new float[] { 1, 1 });

        var leftTextCell = new PdfPCell
        {
            Border = PdfPCell.NO_BORDER,
            HorizontalAlignment = Element.ALIGN_LEFT,
            VerticalAlignment = Element.ALIGN_MIDDLE
        };
        leftTextCell.AddElement(new Paragraph("¡Compromiso con la Democracia!",
            new Font(Font.HELVETICA, 10, Font.BOLD)));
        footerTable.AddCell(leftTextCell);

        try
        {
            _footerBytes ??= DownloadImage("https://i.postimg.cc/76yj8HJ7/Captura.png");
            var footerImage = Image.GetInstance(_footerBytes);
            footerImage.ScaleToFit(150, 150);
            footerTable.AddCell(new PdfPCell(footerImage)
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            });
        }
        catch
        {
            footerTable.AddCell(new PdfPCell { Border = PdfPCell.NO_BORDER });
        }

        footerTable.WriteSelectedRows(0, -1, document.Left, document.Bottom - 10, writer.DirectContent);
    }

    private static byte[] DownloadImage(string url)
    {
        using var client = new HttpClient();
        return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
    }
}
