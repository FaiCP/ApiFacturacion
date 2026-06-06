using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System.Xml.Linq;

namespace Infrastructure.Services;

public class XmlNotaCreditoService : IXmlNotaCreditoService
{
    public string GenerarXml(NotaCredito nc)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement("notaCredito",
                new XAttribute("id", "comprobante"),
                new XAttribute("version", "1.0.0"),

                new XElement("infoTributaria",
                    new XElement("ambiente", (int)nc.Emisor!.Ambiente),
                    new XElement("tipoEmision", "1"),
                    new XElement("razonSocial", nc.Emisor.RazonSocial),
                    new XElement("nombreComercial", nc.Emisor.NombreComercial ?? nc.Emisor.RazonSocial),
                    new XElement("ruc", nc.Emisor.Ruc),
                    new XElement("claveAcceso", nc.ClaveAcceso),
                    new XElement("codDoc", "04"),
                    new XElement("estab", nc.Emisor.SerieEstablecimiento),
                    new XElement("ptoEmi", nc.Emisor.SeriePuntoEmision),
                    new XElement("secuencial", nc.Secuencial),
                    new XElement("dirMatriz", nc.Emisor.Direccion)
                ),

                new XElement("infoNotaCredito",
                    new XElement("fechaEmision", nc.FechaEmision.ToString("dd/MM/yyyy")),
                    new XElement("tipoIdentificacionComprador", GetCodigoTipo(nc.Cliente!.TipoIdentificacion)),
                    new XElement("razonSocialComprador", nc.Cliente.RazonSocial),
                    new XElement("identificacionComprador", nc.Cliente.NumeroIdentificacion),
                    new XElement("obligadoContabilidad", nc.Emisor.ObligadoContabilidad ? "SI" : "NO"),
                    new XElement("codDocModificado", "01"),
                    new XElement("numDocModificado", nc.NumDocModificado),
                    new XElement("fechaEmisionDocSustento", nc.FechaEmisionDocSustento.ToString("dd/MM/yyyy")),
                    new XElement("totalSinImpuestos", nc.TotalSinImpuestos.ToString("F2")),
                    new XElement("valorModificacion", nc.ValorModificacion.ToString("F2")),
                    new XElement("moneda", "DOLAR"),
                    BuildTotalConImpuestos(nc),
                    new XElement("motivo", nc.Motivo)
                ),

                new XElement("detalles",
                    nc.Detalles.Select(d => new XElement("detalle",
                        new XElement("codigoInterno", d.CodigoPrincipal),
                        new XElement("descripcion", d.Descripcion),
                        new XElement("cantidad", d.Cantidad.ToString("F6")),
                        new XElement("precioUnitario", d.PrecioUnitario.ToString("F6")),
                        new XElement("descuento", d.Descuento.ToString("F2")),
                        new XElement("precioTotalSinImpuesto", d.SubtotalSinImpuesto.ToString("F2")),
                        new XElement("impuestos",
                            new XElement("impuesto",
                                new XElement("codigo", "2"),
                                new XElement("codigoPorcentaje", GetCodigoTarifa(d.TarifaIva)),
                                new XElement("tarifa", GetPorcentaje(d.TarifaIva)),
                                new XElement("baseImponible", d.SubtotalSinImpuesto.ToString("F2")),
                                new XElement("valor", d.ValorIva.ToString("F2"))
                            )
                        )
                    ))
                )
            )
        );
        return doc.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement BuildTotalConImpuestos(NotaCredito nc)
    {
        var totalConImpuestos = new XElement("totalConImpuestos");
        var grupos = nc.Detalles.GroupBy(d => d.TarifaIva);
        foreach (var g in grupos)
        {
            totalConImpuestos.Add(new XElement("totalImpuesto",
                new XElement("codigo", "2"),
                new XElement("codigoPorcentaje", GetCodigoTarifa(g.Key)),
                new XElement("baseImponible", g.Sum(d => d.SubtotalSinImpuesto).ToString("F2")),
                new XElement("valor", g.Sum(d => d.ValorIva).ToString("F2"))
            ));
        }
        return totalConImpuestos;
    }

    private static string GetCodigoTipo(TipoIdentificacion t) => t switch
    {
        TipoIdentificacion.Ruc           => "04",
        TipoIdentificacion.Cedula        => "05",
        TipoIdentificacion.Pasaporte     => "06",
        TipoIdentificacion.ConsumidorFinal => "07",
        _ => "07"
    };

    private static string GetCodigoTarifa(TarifaIva t) => t switch
    {
        TarifaIva.Quince => "4",
        TarifaIva.Cero   => "0",
        TarifaIva.Exento => "6",
        _ => "7"
    };

    private static string GetPorcentaje(TarifaIva t) =>
        t == TarifaIva.Quince ? "15.00" : "0.00";
}
