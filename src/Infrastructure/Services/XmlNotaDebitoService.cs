using Domain.Entities;
using Domain.Interfaces;
using System.Xml.Linq;

namespace Infrastructure.Services;

public class XmlNotaDebitoService : IXmlNotaDebitoService
{
    public string GenerarXml(NotaDebito nd)
    {
        var totalImpuestos = nd.Motivos.Sum(m => m.Valor) * 0.15m;

        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement("notaDebito",
                new XAttribute("id", "comprobante"),
                new XAttribute("version", "1.0.0"),

                new XElement("infoTributaria",
                    new XElement("ambiente", (int)nd.Emisor!.Ambiente),
                    new XElement("tipoEmision", "1"),
                    new XElement("razonSocial", nd.Emisor.RazonSocial),
                    new XElement("nombreComercial", nd.Emisor.NombreComercial ?? nd.Emisor.RazonSocial),
                    new XElement("ruc", nd.Emisor.Ruc),
                    new XElement("claveAcceso", nd.ClaveAcceso),
                    new XElement("codDoc", "05"),
                    new XElement("estab", nd.Emisor.SerieEstablecimiento),
                    new XElement("ptoEmi", nd.Emisor.SeriePuntoEmision),
                    new XElement("secuencial", nd.Secuencial),
                    new XElement("dirMatriz", nd.Emisor.Direccion)
                ),

                new XElement("infoNotaDebito",
                    new XElement("fechaEmision", nd.FechaEmision.ToString("dd/MM/yyyy")),
                    new XElement("tipoIdentificacionComprador", GetCodigoTipo(nd.Cliente!.TipoIdentificacion)),
                    new XElement("razonSocialComprador", nd.Cliente.RazonSocial),
                    new XElement("identificacionComprador", nd.Cliente.NumeroIdentificacion),
                    new XElement("obligadoContabilidad", nd.Emisor.ObligadoContabilidad ? "SI" : "NO"),
                    new XElement("codDocModificado", "01"),
                    new XElement("numDocModificado", nd.NumDocModificado),
                    new XElement("fechaEmisionDocSustento", nd.FechaEmisionDocSustento.ToString("dd/MM/yyyy")),
                    new XElement("totalSinImpuestos", nd.TotalSinImpuestos.ToString("F2")),
                    new XElement("impuestos",
                        new XElement("impuesto",
                            new XElement("codigo", "2"),
                            new XElement("codigoPorcentaje", "4"),
                            new XElement("tarifa", "15.00"),
                            new XElement("baseImponible", nd.TotalSinImpuestos.ToString("F2")),
                            new XElement("valor", nd.TotalIva.ToString("F2"))
                        )
                    ),
                    new XElement("valorTotal", nd.ValorTotal.ToString("F2")),
                    new XElement("moneda", "DOLAR"),
                    new XElement("pagos",
                        new XElement("pago",
                            new XElement("formaPago", "01"),
                            new XElement("total", nd.ValorTotal.ToString("F2"))
                        )
                    )
                ),

                new XElement("motivos",
                    nd.Motivos.Select(m => new XElement("motivo",
                        new XElement("razon", m.Razon),
                        new XElement("valor", m.Valor.ToString("F2"))
                    ))
                )
            )
        );
        return doc.ToString(SaveOptions.DisableFormatting);
    }

    private static string GetCodigoTipo(Domain.Enums.TipoIdentificacion t) => t switch
    {
        Domain.Enums.TipoIdentificacion.Ruc            => "04",
        Domain.Enums.TipoIdentificacion.Cedula         => "05",
        Domain.Enums.TipoIdentificacion.Pasaporte      => "06",
        Domain.Enums.TipoIdentificacion.ConsumidorFinal => "07",
        _ => "07"
    };
}
