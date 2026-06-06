using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System.Xml.Linq;

namespace Infrastructure.Services;

/// <summary>
/// Genera XML de factura según esquema SRI Ecuador v1.0.0
/// </summary>
public class XmlFacturaService : IXmlFacturaService
{
    public string GenerarXml(Factura factura)
    {
        var ns = XNamespace.None;
        var fechaEmision = factura.FechaEmision.ToString("dd/MM/yyyy");

        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement("factura",
                new XAttribute("id", "comprobante"),
                new XAttribute("version", "1.0.0"),

                // ── infoTributaria ───────────────────────────────────────
                new XElement("infoTributaria",
                    new XElement("ambiente", (int)factura.Emisor!.Ambiente),
                    new XElement("tipoEmision", "1"),
                    new XElement("razonSocial", factura.Emisor.RazonSocial),
                    new XElement("nombreComercial", factura.Emisor.NombreComercial ?? factura.Emisor.RazonSocial),
                    new XElement("ruc", factura.Emisor.Ruc),
                    new XElement("claveAcceso", factura.ClaveAcceso),
                    new XElement("codDoc", "01"),
                    new XElement("estab", factura.Emisor.SerieEstablecimiento),
                    new XElement("ptoEmi", factura.Emisor.SeriePuntoEmision),
                    new XElement("secuencial", factura.Secuencial),
                    new XElement("dirMatriz", factura.Emisor.Direccion)
                ),

                // ── infoFactura ──────────────────────────────────────────
                new XElement("infoFactura",
                    new XElement("fechaEmision", fechaEmision),
                    new XElement("dirEstablecimiento", factura.Emisor.Direccion),
                    new XElement("obligadoContabilidad", factura.Emisor.ObligadoContabilidad ? "SI" : "NO"),
                    new XElement("tipoIdentificacionComprador", GetCodigoTipoIdentificacion(factura.Cliente!.TipoIdentificacion)),
                    new XElement("razonSocialComprador", factura.Cliente.RazonSocial),
                    new XElement("identificacionComprador", factura.Cliente.NumeroIdentificacion),
                    new XElement("totalSinImpuestos", FormatDecimal(factura.TotalSinImpuestos)),
                    new XElement("totalDescuento", FormatDecimal(factura.TotalDescuento)),
                    new XElement("totalConImpuestos", BuildTotalConImpuestos(factura)),
                    new XElement("propina", "0.00"),
                    new XElement("importeTotal", FormatDecimal(factura.ImporteTotal)),
                    new XElement("moneda", "DOLAR"),
                    new XElement("pagos",
                        new XElement("pago",
                            new XElement("formaPago", "01"),
                            new XElement("total", FormatDecimal(factura.ImporteTotal)),
                            new XElement("plazo", "0"),
                            new XElement("unidadTiempo", "dias")
                        )
                    )
                ),

                // ── detalles ─────────────────────────────────────────────
                new XElement("detalles",
                    factura.Detalles.Select(d => new XElement("detalle",
                        new XElement("codigoPrincipal", d.CodigoPrincipal),
                        new XElement("descripcion", d.Descripcion),
                        new XElement("cantidad", FormatDecimal6(d.Cantidad)),
                        new XElement("precioUnitario", FormatDecimal6(d.PrecioUnitario)),
                        new XElement("descuento", FormatDecimal(d.Descuento)),
                        new XElement("precioTotalSinImpuesto", FormatDecimal(d.SubtotalSinImpuesto)),
                        new XElement("impuestos",
                            BuildImpuestoDetalle(d)
                        )
                    ))
                ),

                // ── infoAdicional ────────────────────────────────────────
                BuildInfoAdicional(factura)
            )
        );

        return doc.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement BuildTotalConImpuestos(Factura factura)
    {
        var grupos = factura.Detalles
            .GroupBy(d => d.TarifaIva)
            .Where(g => g.Key != TarifaIva.NoObjeto && g.Key != TarifaIva.Exento);

        var totalConImpuestos = new XElement("totalConImpuestos");
        foreach (var grupo in grupos)
        {
            var baseImponible = grupo.Sum(d => d.SubtotalSinImpuesto);
            var valorIva = grupo.Sum(d => d.ValorIva);
            totalConImpuestos.Add(new XElement("totalImpuesto",
                new XElement("codigo", "2"),
                new XElement("codigoPorcentaje", GetCodigoTarifa(grupo.Key)),
                new XElement("descuentoAdicional", "0.00"),
                new XElement("baseImponible", FormatDecimal(baseImponible)),
                new XElement("tarifa", GetPorcentajeTarifa(grupo.Key)),
                new XElement("valor", FormatDecimal(valorIva))
            ));
        }

        // Exento e No Objeto no generan IVA pero sí se declaran
        var exento = factura.Detalles.Where(d => d.TarifaIva == TarifaIva.Exento).Sum(d => d.SubtotalSinImpuesto);
        if (exento > 0)
            totalConImpuestos.Add(new XElement("totalImpuesto",
                new XElement("codigo", "2"),
                new XElement("codigoPorcentaje", "6"),
                new XElement("descuentoAdicional", "0.00"),
                new XElement("baseImponible", FormatDecimal(exento)),
                new XElement("tarifa", "0"),
                new XElement("valor", "0.00")
            ));

        return totalConImpuestos;
    }

    private static XElement BuildImpuestoDetalle(DetalleFactura d) =>
        new XElement("impuesto",
            new XElement("codigo", "2"),
            new XElement("codigoPorcentaje", GetCodigoTarifa(d.TarifaIva)),
            new XElement("tarifa", GetPorcentajeTarifa(d.TarifaIva)),
            new XElement("baseImponible", FormatDecimal(d.SubtotalSinImpuesto)),
            new XElement("valor", FormatDecimal(d.ValorIva))
        );

    private static XElement BuildInfoAdicional(Factura factura)
    {
        var info = new XElement("infoAdicional");
        if (!string.IsNullOrEmpty(factura.Cliente?.Email))
            info.Add(new XElement("campoAdicional", new XAttribute("nombre", "email"), factura.Cliente.Email));
        if (!string.IsNullOrEmpty(factura.Cliente?.Telefono))
            info.Add(new XElement("campoAdicional", new XAttribute("nombre", "telefono"), factura.Cliente.Telefono));
        return info;
    }

    private static string GetCodigoTipoIdentificacion(TipoIdentificacion tipo) => tipo switch
    {
        TipoIdentificacion.Ruc           => "04",
        TipoIdentificacion.Cedula        => "05",
        TipoIdentificacion.Pasaporte     => "06",
        TipoIdentificacion.ConsumidorFinal => "07",
        _ => "07"
    };

    private static string GetCodigoTarifa(TarifaIva tarifa) => tarifa switch
    {
        TarifaIva.Cero     => "0",
        TarifaIva.Quince   => "4",   // código SRI para 15% (vigente 2024)
        TarifaIva.Exento   => "6",
        TarifaIva.NoObjeto => "7",
        _ => "4"
    };

    private static string GetPorcentajeTarifa(TarifaIva tarifa) => tarifa switch
    {
        TarifaIva.Quince => "15.00",
        _                => "0.00"
    };

    private static string FormatDecimal(decimal value)  => value.ToString("F2");
    private static string FormatDecimal6(decimal value) => value.ToString("F6");
}
