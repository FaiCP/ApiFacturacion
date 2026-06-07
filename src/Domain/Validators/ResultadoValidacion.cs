namespace Domain.Validators;

/// <summary>
/// Resultado de validación de un comprobante electrónico antes de firmar/enviar al SRI.
/// </summary>
public sealed class ResultadoValidacion
{
    public bool EsValido => Errores.Count == 0;
    public List<string> Errores { get; } = new();

    public void Agregar(string error) => Errores.Add(error);

    public string MensajeConsolidado() => string.Join(" | ", Errores);
}
