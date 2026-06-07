using Domain.Validators;

namespace Domain.Interfaces;

/// <summary>
/// Valida el XML de un comprobante electrónico antes de firmarlo y enviarlo al SRI.
/// Combina validación XSD (si los esquemas están disponibles) con reglas de negocio del SRI.
/// </summary>
public interface IComprobanteValidator
{
    /// <summary>
    /// Valida estructura y reglas de negocio SRI sobre el XML sin firmar.
    /// </summary>
    /// <param name="xml">XML del comprobante (factura, NC, ND o retención).</param>
    ResultadoValidacion Validar(string xml);
}
