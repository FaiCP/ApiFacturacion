using Application.DTOs.Documentos;
using Application.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Infrastructure.Services;

public class PdfService : IPdfService
{
    // GenerarActaHardware — equivalent to HardwareDAL.GenerarActaPDF()
    public byte[] GenerarActaHardware(List<HardwareActaItemDto> items)
    {
        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 36, 36, 106, 100);
        var writer = PdfWriter.GetInstance(document, stream);
        writer.PageEvent = new EncabezadoPageEvent();
        document.Open();

        document.Add(new Paragraph("Informe de Inventarios\n\n",
            new Font(Font.HELVETICA, 16, Font.BOLD)) { Alignment = Element.ALIGN_CENTER });

        var font = new Font(Font.HELVETICA, 9, Font.NORMAL);
        var table = new PdfPTable(12);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 0.5f, 1.4f, 2, 2, 1.5f, 2, 2, 1, 1, 1, 1, 1.4f });

        foreach (var header in new[] { "Nº", "ACTIVO", "CUSTODIO", "DESCRIPCION", "MARCA", "MODELO", "SERIE", "RAM", "DISCO DURO", "PROCESADOR", "VALOR", "ESTADO" })
            table.AddCell(new PdfPCell(new Phrase(header, font)));

        int i = 1;
        foreach (var item in items)
        {
            table.AddCell(new PdfPCell(new Phrase(i++.ToString(), font)));
            table.AddCell(new PdfPCell(new Phrase(item.IdEquipo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.NombreCustodio ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.NombreDispositivo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Marca ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Modelo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.CodigoCne ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Ram ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Rom ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Procesador ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Valor?.ToString() ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Estado ?? "", font)));
        }

        document.Add(table);
        document.Close();
        return stream.ToArray();
    }

    // GenerarActaCustodios — equivalent to CustodiosDAL.GenerarActaPDF()
    public byte[] GenerarActaCustodios(List<CustodioActaItemDto> items)
    {
        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 36, 36, 106, 100);
        var writer = PdfWriter.GetInstance(document, stream);
        writer.PageEvent = new EncabezadoPageEvent();
        document.Open();

        document.Add(new Paragraph("REPORTE GENERAL DE INVENTARIO\n\n",
            new Font(Font.HELVETICA, 16, Font.BOLD)) { Alignment = Element.ALIGN_CENTER });

        var table = new PdfPTable(10);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 1, 2, 1, 2, 2, 2, 2, 2, 2, 1 });

        foreach (var header in new[] { "Nº", "CUSTODIO", "MARCA", "MODELO", "N° SERIE", "COD CNE", "FECHA INGRESO", "VALOR BIEN", "ESTADO", "EQUIPO" })
            table.AddCell(header);

        int i = 1;
        foreach (var item in items)
        {
            table.AddCell(i++.ToString());
            table.AddCell(item.NombreCustodio ?? "");
            table.AddCell(item.Marca ?? "");
            table.AddCell(item.Modelo ?? "");
            table.AddCell(item.CodigoCne ?? "");
            table.AddCell(item.IdEquipo ?? "");
            table.AddCell(item.Fecha.ToString("dd/MM/yyyy"));
            table.AddCell(item.Valor?.ToString() ?? "");
            table.AddCell(item.Estado ?? "");
            table.AddCell(item.NombreDispositivo ?? "");
        }

        document.Add(table);
        document.Close();
        return stream.ToArray();
    }

    // GenerarActaPersonal — equivalent to PersonalDAL.GenerarActaPDF(ids)
    public byte[] GenerarActaPersonal(PersonalActaItemDto persona)
    {
        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 36, 36, 70, 50);
        var writer = PdfWriter.GetInstance(document, stream);
        writer.PageEvent = new EncabezadoPageEvent();
        document.Open();

        document.Add(new Paragraph("ACTA ENTREGA RECEPCIÓN DE CREDENCIALES DIGITALES",
            new Font(Font.HELVETICA, 15, Font.BOLD)) { Alignment = Element.ALIGN_CENTER });

        var fontNormal = new Font(Font.HELVETICA, 12);
        var fontBold = new Font(Font.HELVETICA, 12, Font.BOLD);

        var nombre = new Phrase();
        nombre.Add(new Chunk("La presenté acta de entrega recepción tiene por objeto otorgar credenciales para el manejo del correo Institucional y Quipux a:", fontNormal));
        nombre.Add(new Chunk(persona.Nombre, fontBold));
        nombre.Add(new Chunk(" con número de cedula Nº", fontNormal));
        nombre.Add(new Chunk(persona.Cedula, fontBold));
        nombre.Add(new Chunk($", cuyo cargo es {persona.Cargo} para el proceso Electoral {persona.Fecha:yyyy} El funcionario receptor de las credenciales está obligado al cumplimiento de:\n", fontNormal));
        document.Add(new Paragraph(nombre) { Alignment = Element.ALIGN_JUSTIFIED });

        var lista = new List(List.ORDERED) { IndentationLeft = 20f };
        var item1 = new ListItem("La credencial entregada al funcionario para el manejo del correo Institucional y Quipux, es para uso institucional e intransferible, y su utilización es de exclusiva responsabilidad del funcionario.", fontNormal) { Alignment = Element.ALIGN_JUSTIFIED };
        lista.Add(item1);
        var item2 = new ListItem();
        item2.Add(new Chunk("El funcionario ", fontNormal));
        item2.Add(new Chunk(persona.Nombre, fontBold));
        item2.Add(new Chunk(", se compromete a la no divulgación y buen uso de la información facilitada por la institución con total confidencialidad, de incumplir con este compromiso será responsable de las consecuencias establecidas en el artículo 190.-\"Apropiación fraudulenta por medios electrónicos\" del COIP.", fontNormal));
        item2.Alignment = Element.ALIGN_JUSTIFIED;
        lista.Add(item2);
        lista.Add(new ListItem("En caso de pérdida, olvido o sustracción del usuario y/o clave de acceso para el manejo de correo Institucional y Quipux, el funcionario deberá comunicar al área de tecnología del Consejo Nacional Electoral Delegación Pastaza, de manera inmediata.", fontNormal) { Alignment = Element.ALIGN_JUSTIFIED });
        lista.Add(new ListItem("Las credenciales de acceso serán entregadas de manera personal al funcionario responsable de la misma.", fontNormal) { Alignment = Element.ALIGN_JUSTIFIED });
        document.Add(lista);

        document.Add(new Paragraph($"Para la constancia de lo actuado y en fe de conformidad y aceptación, se suscribe la presente acta en dos originales de igual valor y efecto para las personas que intervienen en esta diligencia, en la ciudad de Puyo, a los {persona.Fecha:dd} días del mes {persona.Fecha:MMMM} del {persona.Fecha:yyyy}\n", fontNormal) { Alignment = Element.ALIGN_JUSTIFIED });

        document.Add(new Paragraph("CUENTA DE CORREO INSTITUCIONAL:\n", new Font(Font.HELVETICA, 15, Font.BOLD)) { Alignment = Element.ALIGN_LEFT });
        document.Add(new Paragraph("LINK: mail.cne.gob.ec", fontNormal));
        document.Add(new Paragraph($"SU CUENTA ES LA SIGUIENTE: {persona.Email}", fontNormal));
        document.Add(new Paragraph($"CONTRASEÑA TEMPORAL: {persona.TempPass}", fontBold));
        document.Add(new Paragraph($"CUENTA DE QUIPUX: ingresará a su cuenta de correo institucional y pinchará en el link que le indica para la generación de la clave de Quipux. {persona.TempPass}", fontNormal));
        document.Add(new Paragraph("El link para el ingreso a QUIPUX es el siguiente: quipux.cne.gob.ec, con su número de cédula y la contraseña que Ud. genere personalmente.\n\n", fontNormal));

        var tableRef = new PdfPTable(1) { WidthPercentage = 100 };
        tableRef.AddCell("RECIBIDO POR");
        document.Add(tableRef);

        var table = new PdfPTable(3) { WidthPercentage = 100 };
        table.AddCell("NOMBRE"); table.AddCell("CARGO"); table.AddCell("FIRMA");
        table.AddCell(persona.Nombre); table.AddCell(persona.Cargo); table.AddCell(" ");
        document.Add(table);

        var tableEntref = new PdfPTable(1) { WidthPercentage = 100 };
        tableEntref.AddCell("ENTREGADO POR");
        document.Add(tableEntref);

        var table2 = new PdfPTable(3) { WidthPercentage = 100 };
        table2.AddCell("ING. NELSON RICARDO CARDENAS HERMOZA\n\n");
        table2.AddCell("Técnico Electoral 2\n\n");
        table2.AddCell(" ");
        document.Add(table2);

        document.Close();
        return stream.ToArray();
    }

    // GenerarReportePersonalAsync — equivalent to PersonalDAL.DescargarPDF()
    public async Task<byte[]> GenerarReportePersonalAsync(List<ReportePersonalItemDto> items)
    {
        using var stream = new MemoryStream();
        var pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);
        var writer = PdfWriter.GetInstance(pdfDoc, stream);
        pdfDoc.Open();

        // Download logo
        Image? logo = null;
        try
        {
            using var client = new HttpClient();
            var logoBytes = await client.GetByteArrayAsync("https://i.postimg.cc/BQM89b61/logo2.png");
            using var logoStream = new MemoryStream(logoBytes);
            logo = Image.GetInstance(logoStream);
            logo.ScaleToFit(80, 80);
        }
        catch { }

        // Header table
        var mainHeaderTable = new PdfPTable(2) { WidthPercentage = 100 };
        mainHeaderTable.SetWidths(new float[] { 1, 3 });

        var logoTable = new PdfPTable(1) { WidthPercentage = 100 };
        if (logo != null)
        {
            logoTable.AddCell(new PdfPCell(logo) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 });
        }
        else
        {
            logoTable.AddCell(new PdfPCell { Border = PdfPCell.NO_BORDER });
        }
        mainHeaderTable.AddCell(new PdfPCell(logoTable) { Border = PdfPCell.NO_BORDER });

        var textTable = new PdfPTable(1) { WidthPercentage = 100 };
        textTable.AddCell(new PdfPCell(new Phrase("Reporte de entrega y recepción de equipos informáticos y sistemas electorales para usuarios finales en el ámbito provincial", new Font(Font.HELVETICA, 12, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 });

        var now = DateTime.Now;
        var primerMes = new DateTime(now.Year, 1, 1);
        var segundoMes = primerMes.AddMonths(5);
        var cultura = new System.Globalization.CultureInfo("es-ES");
        var rangoMeses = $"{primerMes.ToString("MMMM", cultura)} - {segundoMes.ToString("MMMM", cultura)} {now.Year}";
        textTable.AddCell(new PdfPCell(new Phrase($"REGISTRO DE ACTAS ENTREGA RECEPCIÓN\n{rangoMeses}", new Font(Font.HELVETICA, 12, Font.BOLD))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 5 });
        mainHeaderTable.AddCell(new PdfPCell(textTable) { Border = PdfPCell.NO_BORDER });
        pdfDoc.Add(mainHeaderTable);

        // Content table
        var table = new PdfPTable(6) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1.2f, 3f, 3f, 0.9f, 0.98f, 2f });

        var fontBold = new Font(Font.HELVETICA, 10, Font.BOLD);
        var fontSmall = new Font(Font.HELVETICA, 11, Font.NORMAL);

        foreach (var header in new[] { "Fecha", "Entrega", "Recibe", "Equipos", "Sistemas", "Observación" })
            table.AddCell(new PdfPCell(new Phrase(header, fontBold)) { BackgroundColor = new BaseColor(192, 192, 192), HorizontalAlignment = Element.ALIGN_CENTER });

        foreach (var item in items)
        {
            table.AddCell(new PdfPCell(new Phrase(item.Fecha?.ToString("yyyy-MM-dd") ?? "", fontSmall)));
            table.AddCell(new PdfPCell(new Phrase(item.Entrega ?? "", fontSmall)));
            table.AddCell(new PdfPCell(new Phrase(item.Recibe ?? "", fontSmall)));
            table.AddCell(new PdfPCell(new Phrase(item.EquiposE ?? "", fontSmall)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(item.EquiposP ?? "", fontSmall)) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(item.Observacion ?? "", fontSmall)));
        }
        pdfDoc.Add(table);

        // Footer table
        var elaboradoTable = new PdfPTable(2) { WidthPercentage = 100 };
        elaboradoTable.SetWidths(new float[] { 2, 1 });

        var elaboradoCell = new PdfPCell { Border = PdfPCell.BOX, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, Padding = 10 };
        var contenido = new Paragraph { Alignment = Element.ALIGN_CENTER };
        contenido.Add(new Chunk("ELABORADO POR:\n", new Font(Font.HELVETICA, 12, Font.BOLD)));
        contenido.Add(new Chunk("ING. RICARDO CARDENAS H.\n", new Font(Font.HELVETICA, 12)));
        contenido.Add(new Chunk("TECNICO ELECTORAL 2\n", new Font(Font.HELVETICA, 12)));
        contenido.Add(new Chunk("Unidad Provincial de Seguridad Informática y Proyectos Tecnológicos Electorales\n", new Font(Font.HELVETICA, 12)));
        contenido.Add(new Chunk("de Tungurahua", new Font(Font.HELVETICA, 12)));
        elaboradoCell.AddElement(contenido);
        elaboradoTable.AddCell(elaboradoCell);
        elaboradoTable.AddCell(new PdfPCell(new Phrase(" ")) { Border = PdfPCell.BOX, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Padding = 10 });
        pdfDoc.Add(elaboradoTable);

        pdfDoc.Close();
        return stream.ToArray();
    }

    // GenerarActaKits — equivalent to KITSDAL.GenerarActaPDF()
    public byte[] GenerarActaKits(List<KitActaItemDto> items)
    {
        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 36, 36, 106, 100);
        var writer = PdfWriter.GetInstance(document, stream);
        writer.PageEvent = new EncabezadoPageEvent();
        document.Open();

        document.Add(new Paragraph("Informe de Inventarios\n\n",
            new Font(Font.HELVETICA, 16, Font.BOLD)) { Alignment = Element.ALIGN_CENTER });

        var font = new Font(Font.HELVETICA, 9, Font.NORMAL);
        var table = new PdfPTable(8);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 0.5f, 1.4f, 2, 2, 1.5f, 2, 2, 1.4f });

        foreach (var header in new[] { "Nº", "INSUMO", "MARCA", "MODELO", "Serie", "CANTIDAD", "ESTADO", "OBSERVACION" })
            table.AddCell(new PdfPCell(new Phrase(header, font)));

        int i = 1;
        foreach (var item in items)
        {
            table.AddCell(new PdfPCell(new Phrase(i++.ToString(), font)));
            table.AddCell(new PdfPCell(new Phrase(item.Insumo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Marca ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Modelo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Serie ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Cantidad ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Estado ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Observacion ?? "", font)));
        }

        document.Add(table);
        document.Close();
        return stream.ToArray();
    }

    // GenerarActaGestionActivos — equivalent to GestionActuvosDAL.GenerarActaPDF(ids)
    public byte[] GenerarActaGestionActivos(GestionActivoActaItemDto primerEquipo, List<GestionActivoActaItemDto> items)
    {
        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 36, 36, 106, 100);
        var writer = PdfWriter.GetInstance(document, stream);
        writer.PageEvent = new EncabezadoPageEvent();
        document.Open();

        document.Add(new Paragraph("ACTA ENTREGA-RECEPCIÓN\n\n",
            new Font(Font.HELVETICA, 16, Font.BOLD)) { Alignment = Element.ALIGN_CENTER });

        document.Add(new Paragraph(
            $"En la ciudad de Puyo, a los {primerEquipo.Fecha:dd} días del mes de {primerEquipo.Fecha:MMMM} del año {primerEquipo.Fecha:yyyy}, en las instalaciones de la Delegación Provincial Electoral del Consejo Nacional Electoral - Pastaza, comparecen por una parte el ING.NELSON RICARDO CARDENAS HERMOSA - TECNICO ELECTORAL 2- y por otra parte EL ING. {primerEquipo.NombreCustodio}- {primerEquipo.Cargo}, quienes proceden con la entrega recepción de equipos informáticos propiedad de la Delegación Provincial Electoral de Pastaza, conforme al siguiente detalle:\n\n",
            new Font(Font.HELVETICA, 12)));

        var font = new Font(Font.HELVETICA, 10, Font.NORMAL);
        var table = new PdfPTable(7);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 0.5f, 1.6f, 1.5f, 2, 2, 1.5f, 1.4f });

        foreach (var header in new[] { "Nº", "DESCRIPCION", "MARCA", "MODELO", "SERIE", "ACTIVO", "ESTADO" })
            table.AddCell(new PdfPCell(new Phrase(header, font)));

        int i = 1;
        foreach (var item in items)
        {
            table.AddCell(new PdfPCell(new Phrase(i++.ToString(), font)));
            table.AddCell(new PdfPCell(new Phrase(item.NombreDispositivo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Marca ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Modelo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.CodigoCne ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.IdEquipo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Estado ?? "", font)));
        }
        document.Add(table);

        var firmasTable = new PdfPTable(2) { WidthPercentage = 100 };
        firmasTable.SetWidths(new float[] { 2, 2 });

        var cell1 = new PdfPCell { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
        cell1.AddElement(new Paragraph("\nENTREGO CONFORME:\n\n", new Font(Font.HELVETICA, 12)));
        cell1.AddElement(new Paragraph("\n\n\n\nING. NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL 2\r\n\r\n", new Font(Font.HELVETICA, 12)));
        firmasTable.AddCell(cell1);

        var cell2 = new PdfPCell { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
        cell2.AddElement(new Paragraph("\nRECIBO CONFORME:\n\n", new Font(Font.HELVETICA, 12)));
        cell2.AddElement(new Paragraph($"\n\n\n\n{primerEquipo.NombreCustodio} - {primerEquipo.Cargo}\n", new Font(Font.HELVETICA, 12)));
        firmasTable.AddCell(cell2);

        document.Add(firmasTable);
        document.Close();
        return stream.ToArray();
    }

    // GenerarDevolucionGestionActivos — equivalent to GestionActuvosDAL.GenerarDevolucionPDF(ids)
    public byte[] GenerarDevolucionGestionActivos(GestionActivoActaItemDto primerEquipo, List<GestionActivoActaItemDto> items)
    {
        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 36, 36, 106, 100);
        var writer = PdfWriter.GetInstance(document, stream);
        writer.PageEvent = new EncabezadoPageEvent();
        document.Open();

        document.Add(new Paragraph("ACTA ENTREGA-RECEPCIÓN \n\n",
            new Font(Font.HELVETICA, 16, Font.BOLD)) { Alignment = Element.ALIGN_CENTER });

        document.Add(new Paragraph(
            $"En la ciudad de Puyo, a los {primerEquipo.Fecha:dd} días del mes de {primerEquipo.Fecha:MMMM} del año {primerEquipo.Fecha:yyyy}, en las instalaciones de la Delegación Provincial Electoral del Consejo Nacional Electoral - Pastaza, comparecen por una parte el ING.NELSON RICARDO CARDENAS HERMOSA - TECNICO ELECTORAL 2  y por otra parte EL ING. {primerEquipo.NombreCustodio}- RESPONSABLE DE {primerEquipo.Cargo}, quienes proceden con la entrega recepción de equipos informáticos propiedad de la Delegación Provincial Electoral de Pastaza, conforme al siguiente detalle:\n\n",
            new Font(Font.HELVETICA, 12)));

        var font = new Font(Font.HELVETICA, 10, Font.NORMAL);
        var table = new PdfPTable(7);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 0.5f, 1.4f, 2, 2, 1.5f, 2, 1.4f });

        foreach (var header in new[] { "Nº", "DESCRIPCION", "MARCA", "MODELO", "SERIE", "ACTIVO", "ESTADO" })
            table.AddCell(new PdfPCell(new Phrase(header, font)));

        int i = 1;
        foreach (var item in items)
        {
            table.AddCell(new PdfPCell(new Phrase(i++.ToString(), font)));
            table.AddCell(new PdfPCell(new Phrase(item.NombreDispositivo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Marca ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Modelo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.CodigoCne ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.IdEquipo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Estado ?? "", font)));
        }
        document.Add(table);

        var firmasTable = new PdfPTable(2) { WidthPercentage = 100 };
        firmasTable.SetWidths(new float[] { 2, 2 });

        var cell1 = new PdfPCell { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
        cell1.AddElement(new Paragraph("\nENTREGO CONFORME:\n\n", new Font(Font.HELVETICA, 12)));
        cell1.AddElement(new Paragraph($"\n\n\n\n{primerEquipo.NombreCustodio} - {primerEquipo.Cargo}\r\n\r\n", new Font(Font.HELVETICA, 12)));
        firmasTable.AddCell(cell1);

        var cell2 = new PdfPCell { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
        cell2.AddElement(new Paragraph("\nRECIBO CONFORME:\n\n", new Font(Font.HELVETICA, 12)));
        cell2.AddElement(new Paragraph("\n\n\n\nING. NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL 2\n", new Font(Font.HELVETICA, 12)));
        firmasTable.AddCell(cell2);

        document.Add(firmasTable);
        document.Close();
        return stream.ToArray();
    }

    // GenerarActaHistorialPrestamos — equivalent to HistorialPrestamosDAL.GenerarActaPDF(ids)
    public byte[] GenerarActaHistorialPrestamos(HistorialActaItemDto primerEquipo, List<HistorialActaItemDto> items)
    {
        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 36, 36, 106, 100);
        var writer = PdfWriter.GetInstance(document, stream);
        writer.PageEvent = new EncabezadoPageEvent();
        document.Open();

        document.Add(new Paragraph("REPORTE DE BIENES POR CUSTODIO\n\n",
            new Font(Font.HELVETICA, 16, Font.BOLD)) { Alignment = Element.ALIGN_CENTER });

        document.Add(new Paragraph($"CUSTODIO: {primerEquipo.NombreCustodio}\n\n", new Font(Font.HELVETICA, 12)));
        document.Add(new Paragraph($"DEPARTAMENTO: {primerEquipo.Departamento}\n\n", new Font(Font.HELVETICA, 12)));

        var font = new Font(Font.HELVETICA, 10, Font.NORMAL);
        var table = new PdfPTable(9);
        table.WidthPercentage = 100;
        table.SetWidths(new float[] { 0.5f, 1.4f, 2, 2, 1.5f, 2, 2, 1, 1.4f });

        foreach (var header in new[] { "Nº", "CÓDIGO ACTUAL", "BIEN", "SERIE", "MODELO", "MARCA", "VALOR", "ESTADO", "OBSERVACIONES" })
            table.AddCell(new PdfPCell(new Phrase(header, font)));

        int i = 1;
        foreach (var item in items)
        {
            table.AddCell(new PdfPCell(new Phrase(i++.ToString(), font)));
            table.AddCell(new PdfPCell(new Phrase(item.IdEquipo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.NombreDispositivo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.CodigoCne ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Modelo ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Marca ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Valor?.ToString() ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Estado ?? "", font)));
            table.AddCell(new PdfPCell(new Phrase(item.Descripcion ?? "", font)));
        }
        document.Add(table);

        var firmasTable = new PdfPTable(2) { WidthPercentage = 100 };
        firmasTable.SetWidths(new float[] { 75, 25 });

        var delegadosCell = new PdfPCell { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT };
        delegadosCell.AddElement(new Paragraph("\n\n\n\nRICARDO CARDENAS HERMOZA\r\nDelegado de la Dirección Provincial", new Font(Font.HELVETICA, 12)));
        delegadosCell.AddElement(new Paragraph("\n\n\nHOMERO CABRERA MEZA\r\nDelegado Unidad Provincial Administrativa", new Font(Font.HELVETICA, 12)));
        delegadosCell.AddElement(new Paragraph("\n\n\nRUTH ELIZABETH RONDAL LLUGAY\r\nDelegada Unidad Provincial Financiera", new Font(Font.HELVETICA, 12)));
        firmasTable.AddCell(delegadosCell);

        var custodioCell = new PdfPCell { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT };
        custodioCell.AddElement(new Paragraph($"\n\n\n\n{primerEquipo.NombreCustodio} - {primerEquipo.Cargo}\n", new Font(Font.HELVETICA, 12)));
        firmasTable.AddCell(custodioCell);

        document.Add(firmasTable);
        document.Close();
        return stream.ToArray();
    }
}
