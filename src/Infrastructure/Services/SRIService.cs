using Domain.Enums;
using Domain.Interfaces;
using System.Text;
using System.Xml;

namespace Infrastructure.Services;

/// <summary>
/// Comunicación SOAP con el SRI Ecuador — recepción y autorización de comprobantes
/// </summary>
public class SRIService : ISRIService
{
    private readonly HttpClient _httpClient;

    // Endpoints SRI
    private static readonly Dictionary<AmbienteSRI, string> UrlsRecepcion = new()
    {
        [AmbienteSRI.Pruebas]    = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline",
        [AmbienteSRI.Produccion] = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline"
    };

    private static readonly Dictionary<AmbienteSRI, string> UrlsAutorizacion = new()
    {
        [AmbienteSRI.Pruebas]    = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline",
        [AmbienteSRI.Produccion] = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline"
    };

    public SRIService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("SRI");
    }

    public async Task<SRIRecepcionResult> EnviarComprobanteAsync(string xmlFirmado, AmbienteSRI ambiente)
    {
        var xmlB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlFirmado));

        var soapEnvelope = $"""
            <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                              xmlns:ec="http://ec.gob.sri.ws.recepcion">
              <soapenv:Header/>
              <soapenv:Body>
                <ec:validarComprobante>
                  <xml>{xmlB64}</xml>
                </ec:validarComprobante>
              </soapenv:Body>
            </soapenv:Envelope>
            """;

        try
        {
            var response = await EnviarSoapAsync(UrlsRecepcion[ambiente], soapEnvelope);
            return ParseRespuestaRecepcion(response);
        }
        catch (Exception ex)
        {
            return new SRIRecepcionResult(false, $"Error de conexión con el SRI: {ex.Message}", []);
        }
    }

    public async Task<SRIAutorizacionResult> ConsultarAutorizacionAsync(string claveAcceso, AmbienteSRI ambiente)
    {
        var soapEnvelope = $"""
            <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                              xmlns:ec="http://ec.gob.sri.ws.autorizacion">
              <soapenv:Header/>
              <soapenv:Body>
                <ec:autorizacionComprobante>
                  <claveAccesoComprobante>{claveAcceso}</claveAccesoComprobante>
                </ec:autorizacionComprobante>
              </soapenv:Body>
            </soapenv:Envelope>
            """;

        try
        {
            var response = await EnviarSoapAsync(UrlsAutorizacion[ambiente], soapEnvelope);
            return ParseRespuestaAutorizacion(response);
        }
        catch (Exception ex)
        {
            return new SRIAutorizacionResult(EstadoSRI.ENVIADA, null, null, $"Error consultando autorización: {ex.Message}");
        }
    }

    private async Task<string> EnviarSoapAsync(string url, string soapEnvelope)
    {
        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "");
        var response = await _httpClient.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    private static SRIRecepcionResult ParseRespuestaRecepcion(string responseXml)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(responseXml);
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

            var estado = doc.SelectSingleNode("//estado", ns)?.InnerText?.Trim();
            var mensajes = doc.SelectNodes("//mensaje")!
                .Cast<XmlNode>()
                .Select(m => SriErrorTranslator.Traducir(m.SelectSingleNode("identificador")?.InnerText ?? "", m.SelectSingleNode("mensaje")?.InnerText ?? ""))
                .ToList();

            var exitoso = estado?.Equals("RECIBIDA", StringComparison.OrdinalIgnoreCase) == true;
            return new SRIRecepcionResult(exitoso, estado ?? "DESCONOCIDO", mensajes.AsReadOnly());
        }
        catch
        {
            return new SRIRecepcionResult(false, "No se pudo interpretar la respuesta del SRI.", []);
        }
    }

    private static SRIAutorizacionResult ParseRespuestaAutorizacion(string responseXml)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(responseXml);

            var estadoNode = doc.SelectSingleNode("//autorizacion/estado")?.InnerText?.Trim();
            var numeroAut  = doc.SelectSingleNode("//autorizacion/numeroAutorizacion")?.InnerText?.Trim();
            var fechaAutStr = doc.SelectSingleNode("//autorizacion/fechaAutorizacion")?.InnerText?.Trim();
            var mensajesNodos = doc.SelectNodes("//autorizacion/mensajes/mensaje");

            DateTime? fechaAut = null;
            if (DateTime.TryParse(fechaAutStr, out var fa)) fechaAut = fa;

            var motivoRechazo = mensajesNodos?.Cast<XmlNode>()
                .Where(m => m.SelectSingleNode("tipo")?.InnerText?.Trim() == "ERROR")
                .Select(m => SriErrorTranslator.Traducir(
                    m.SelectSingleNode("identificador")?.InnerText ?? "",
                    m.SelectSingleNode("mensaje")?.InnerText ?? ""))
                .FirstOrDefault();

            var estado = estadoNode switch
            {
                "AUTORIZADO"  => EstadoSRI.AUTORIZADA,
                "NO AUTORIZADO" or "RECHAZADO" => EstadoSRI.RECHAZADA,
                _ => EstadoSRI.ENVIADA
            };

            return new SRIAutorizacionResult(estado, numeroAut, fechaAut, motivoRechazo);
        }
        catch
        {
            return new SRIAutorizacionResult(EstadoSRI.ENVIADA, null, null, "No se pudo interpretar la respuesta de autorización.");
        }
    }
}
