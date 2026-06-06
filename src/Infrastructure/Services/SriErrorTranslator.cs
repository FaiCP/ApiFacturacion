namespace Infrastructure.Services;

/// <summary>
/// Traduce códigos de error SRI a mensajes legibles para el usuario
/// </summary>
public static class SriErrorTranslator
{
    private static readonly Dictionary<string, string> Mensajes = new()
    {
        ["45"]   = "El RUC del emisor no está activo en el SRI.",
        ["47"]   = "El certificado digital ha expirado o no es válido.",
        ["48"]   = "La firma digital del comprobante no es válida.",
        ["49"]   = "La clave de acceso ya fue utilizada anteriormente.",
        ["64"]   = "La fecha de emisión no corresponde al período autorizado.",
        ["65"]   = "El número de comprobante no es secuencial.",
        ["70"]   = "El RUC del receptor no existe en el SRI.",
        ["43"]   = "El ambiente del comprobante no coincide con el ambiente de recepción.",
        ["44"]   = "El tipo de comprobante no es válido.",
        ["60"]   = "El total del comprobante es incorrecto.",
        ["61"]   = "Los valores de IVA son incorrectos.",
        ["AUTH-CERT-0001"] = "Certificado digital no válido. Verifique que el archivo .p12 sea correcto.",
        ["AUTH-CERT-0023"] = "El certificado digital ha caducado. Renuévelo con su proveedor.",
        ["FIRMA_INVALIDA"]  = "La firma digital es inválida. Verifique su certificado.",
        ["CLAVE_DUPLICADA"] = "Esta clave de acceso ya fue autorizada anteriormente.",
    };

    public static string Traducir(string codigo, string mensajeSRI)
    {
        if (Mensajes.TryGetValue(codigo, out var traduccion))
            return $"{traduccion} (código SRI: {codigo})";

        // Retornar mensaje original si no hay traducción
        return string.IsNullOrWhiteSpace(mensajeSRI) ? $"Error SRI código {codigo}" : mensajeSRI;
    }
}
