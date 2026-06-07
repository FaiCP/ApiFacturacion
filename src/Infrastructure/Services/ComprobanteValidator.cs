using Domain.Interfaces;
using Domain.Validators;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Infrastructure.Services;

/// <summary>
/// Validador de comprobantes electrónicos SRI previo a la firma.
/// Tres capas: (1) XML bien formado, (2) XSD oficial si está disponible en /schemas,
/// (3) reglas de negocio del SRI que atrapan los rechazos más frecuentes.
/// </summary>
public class ComprobanteValidator : IComprobanteValidator
{
    // Mapa codDoc -> nombre de archivo XSD esperado en el directorio de esquemas
    private static readonly Dictionary<string, string> SchemaPorCodDoc = new()
    {
        ["01"] = "factura.xsd",
        ["04"] = "notaCredito.xsd",
        ["05"] = "notaDebito.xsd",
        ["06"] = "guiaRemision.xsd",
        ["07"] = "comprobanteRetencion.xsd",
    };

    private readonly string _schemasDir;

    public ComprobanteValidator()
    {
        // Esquemas oficiales del SRI (opcionales). Si no existen, se omite la capa XSD.
        _schemasDir = Path.Combine(AppContext.BaseDirectory, "schemas");
    }

    public ResultadoValidacion Validar(string xml)
    {
        var resultado = new ResultadoValidacion();

        // ── Capa 1: XML bien formado ──────────────────────────────────────
        XDocument doc;
        try
        {
            doc = XDocument.Parse(xml);
        }
        catch (XmlException ex)
        {
            resultado.Agregar($"XML mal formado: {ex.Message}");
            return resultado; // sin XML válido no se puede continuar
        }

        var info = doc.Root?.Element("infoTributaria");
        if (info is null)
        {
            resultado.Agregar("Falta el bloque infoTributaria.");
            return resultado;
        }

        var codDoc = info.Element("codDoc")?.Value ?? "";

        // ── Capa 2: validación XSD (si el esquema está disponible) ─────────
        ValidarContraXsd(xml, codDoc, resultado);

        // ── Capa 3: reglas de negocio SRI ─────────────────────────────────
        ValidarReglasNegocio(info, resultado);

        return resultado;
    }

    private void ValidarContraXsd(string xml, string codDoc, ResultadoValidacion resultado)
    {
        if (!SchemaPorCodDoc.TryGetValue(codDoc, out var schemaFile))
            return; // codDoc desconocido: lo cubren las reglas de negocio

        var schemaPath = Path.Combine(_schemasDir, schemaFile);
        if (!File.Exists(schemaPath))
            return; // esquema no provisto: se omite esta capa silenciosamente

        try
        {
            var schemas = new XmlSchemaSet();
            schemas.Add(null, schemaPath);

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemas,
            };
            settings.ValidationEventHandler += (_, e) =>
                resultado.Agregar($"XSD ({e.Severity}): {e.Message}");

            using var reader = XmlReader.Create(new StringReader(xml), settings);
            while (reader.Read()) { /* dispara ValidationEventHandler */ }
        }
        catch (XmlSchemaException ex)
        {
            resultado.Agregar($"Error cargando esquema XSD {schemaFile}: {ex.Message}");
        }
    }

    private static void ValidarReglasNegocio(XElement info, ResultadoValidacion resultado)
    {
        var clave = info.Element("claveAcceso")?.Value ?? "";
        var ruc = info.Element("ruc")?.Value ?? "";
        var ambiente = info.Element("ambiente")?.Value ?? "";
        var codDoc = info.Element("codDoc")?.Value ?? "";
        var estab = info.Element("estab")?.Value ?? "";
        var ptoEmi = info.Element("ptoEmi")?.Value ?? "";
        var secuencial = info.Element("secuencial")?.Value ?? "";
        var razonSocial = info.Element("razonSocial")?.Value ?? "";

        // Campos obligatorios
        if (string.IsNullOrWhiteSpace(razonSocial)) resultado.Agregar("razonSocial vacío.");
        if (ruc.Length != 13 || !EsNumerico(ruc)) resultado.Agregar($"RUC inválido: '{ruc}' (debe tener 13 dígitos).");
        if (ambiente is not ("1" or "2")) resultado.Agregar($"Ambiente inválido: '{ambiente}' (1=Pruebas, 2=Producción).");
        if (estab.Length != 3 || !EsNumerico(estab)) resultado.Agregar($"estab inválido: '{estab}' (3 dígitos).");
        if (ptoEmi.Length != 3 || !EsNumerico(ptoEmi)) resultado.Agregar($"ptoEmi inválido: '{ptoEmi}' (3 dígitos).");
        if (secuencial.Length != 9 || !EsNumerico(secuencial)) resultado.Agregar($"secuencial inválido: '{secuencial}' (9 dígitos).");

        // Clave de acceso: 49 dígitos numéricos
        if (clave.Length != 49 || !EsNumerico(clave))
        {
            resultado.Agregar($"claveAcceso inválida: longitud {clave.Length} (debe ser 49 dígitos numéricos).");
            return; // sin clave válida no se pueden hacer las verificaciones cruzadas
        }

        // Dígito verificador módulo 11
        var clave48 = clave[..48];
        var verificadorEsperado = CalcularDigitoVerificador(clave48);
        var verificadorReal = clave[48] - '0';
        if (verificadorEsperado != verificadorReal)
            resultado.Agregar($"Dígito verificador de claveAcceso incorrecto (esperado {verificadorEsperado}, encontrado {verificadorReal}).");

        // Coherencia clave de acceso ↔ infoTributaria (rechazo #1 del SRI)
        // Estructura: [fecha 8][tipo 2][ruc 13][ambiente 1][estab 3][ptoEmi 3][secuencial 9][cod 8][tipoEmision 1][verif 1]
        var codDocEnClave = clave.Substring(8, 2);
        var rucEnClave = clave.Substring(10, 13);
        var ambienteEnClave = clave.Substring(23, 1);
        var estabEnClave = clave.Substring(24, 3);
        var ptoEmiEnClave = clave.Substring(27, 3);
        var secuencialEnClave = clave.Substring(30, 9);

        if (codDocEnClave != codDoc) resultado.Agregar($"codDoc no coincide: infoTributaria='{codDoc}' vs claveAcceso='{codDocEnClave}'.");
        if (rucEnClave != ruc) resultado.Agregar($"RUC no coincide: infoTributaria='{ruc}' vs claveAcceso='{rucEnClave}'.");
        if (ambienteEnClave != ambiente) resultado.Agregar($"Ambiente no coincide: infoTributaria='{ambiente}' vs claveAcceso='{ambienteEnClave}'.");
        if (estabEnClave != estab) resultado.Agregar($"estab no coincide: infoTributaria='{estab}' vs claveAcceso='{estabEnClave}'.");
        if (ptoEmiEnClave != ptoEmi) resultado.Agregar($"ptoEmi no coincide: infoTributaria='{ptoEmi}' vs claveAcceso='{ptoEmiEnClave}'.");
        if (secuencialEnClave != secuencial) resultado.Agregar($"secuencial no coincide: infoTributaria='{secuencial}' vs claveAcceso='{secuencialEnClave}'.");
    }

    private static bool EsNumerico(string s) => s.Length > 0 && s.All(char.IsAsciiDigit);

    // Mismo algoritmo que ClaveAccesoGenerator (pesos 2..7, módulo 11)
    private static int CalcularDigitoVerificador(string clave48)
    {
        int[] pesos = [2, 3, 4, 5, 6, 7];
        int suma = 0;
        for (int i = clave48.Length - 1; i >= 0; i--)
        {
            int peso = pesos[(clave48.Length - 1 - i) % pesos.Length];
            suma += (clave48[i] - '0') * peso;
        }
        int residuo = suma % 11;
        return residuo switch { 0 => 0, 1 => 1, _ => 11 - residuo };
    }
}
