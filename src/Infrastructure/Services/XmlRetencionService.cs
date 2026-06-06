using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System.Xml.Linq;

namespace Infrastructure.Services;

public class XmlRetencionService : IXmlRetencionService
{
    public string GenerarXml(Retencion ret)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement("comprobanteRetencion",
                new XAttribute("id", "comprobante"),
                new XAttribute("version", "1.0.0"),

                new XElement("infoTributaria",
                    new XElement("ambiente", (int)ret.Emisor!.Ambiente),
                    new XElement("tipoEmision", "1"),
                    new XElement("razonSocial", ret.Emisor.RazonSocial),
                    new XElement("nombreComercial", ret.Emisor.NombreComercial ?? ret.Emisor.RazonSocial),
                    new XElement("ruc", ret.Emisor.Ruc),
                    new XElement("claveAcceso", ret.ClaveAcceso),
                    new XElement("codDoc", "07"),
                    new XElement("estab", ret.Emisor.SerieEstablecimiento),
                    new XElement("ptoEmi", ret.Emisor.SeriePuntoEmision),
                    new XElement("secuencial", ret.Secuencial),
                    new XElement("dirMatriz", ret.Emisor.Direccion)
                ),

                new XElement("infoCompRetencion",
                    new XElement("fechaEmision", ret.FechaEmision.ToString("dd/MM/yyyy")),
                    new XElement("dirEstablecimiento", ret.Emisor.Direccion),
                    new XElement("obligadoContabilidad", ret.Emisor.ObligadoContabilidad ? "SI" : "NO"),
                    new XElement("tipoIdentificacionSujetoRetenido", GetCodigoTipo(ret.SujetoRetenido!.TipoIdentificacion)),
                    new XElement("razonSocialSujetoRetenido", ret.SujetoRetenido.RazonSocial),
                    new XElement("identificacionSujetoRetenido", ret.SujetoRetenido.NumeroIdentificacion),
                    new XElement("periodoFiscal", ret.PeriodoFiscal)
                ),

                new XElement("impuestos",
                    ret.Detalles.Select(d => new XElement("impuesto",
                        new XElement("codigo", ((int)d.TipoImpuesto).ToString()),
                        new XElement("codigoRetencion", d.CodigoRetencion),
                        new XElement("baseImponible", d.BaseImponible.ToString("F2")),
                        new XElement("porcentajeRetener", d.PorcentajeRetener.ToString("F2")),
                        new XElement("valorRetenido", d.ValorRetenido.ToString("F2")),
                        new XElement("codDocSustento", d.CodDocSustento),
                        new XElement("numDocSustento", d.NumDocSustento),
                        new XElement("fechaEmisionDocSustento", d.FechaEmisionDocSustento.ToString("dd/MM/yyyy"))
                    ))
                )
            )
        );
        return doc.ToString(SaveOptions.DisableFormatting);
    }

    private static string GetCodigoTipo(TipoIdentificacion t) => t switch
    {
        TipoIdentificacion.Ruc            => "04",
        TipoIdentificacion.Cedula         => "05",
        TipoIdentificacion.Pasaporte      => "06",
        TipoIdentificacion.ConsumidorFinal => "07",
        _ => "07"
    };
}
