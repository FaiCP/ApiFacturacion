using Domain.Enums;

namespace Domain.Validators;

/// <summary>
/// Genera clave de acceso SRI de 49 dígitos.
/// Estructura: [fecha 8][tipo 2][ruc 13][ambiente 1][serie 6][secuencial 9][codigoNumerico 8][tipoEmision 1][verificador 1]
/// </summary>
public static class ClaveAccesoGenerator
{
    public static string Generar(
        DateTime fecha,
        TipoDocumentoSRI tipoDocumento,
        string ruc,
        AmbienteSRI ambiente,
        string serieEstablecimiento,
        string seriePuntoEmision,
        int secuencial,
        int? codigoNumerico = null)
    {
        string fechaStr = fecha.ToString("ddMMyyyy");
        string tipoStr = ((int)tipoDocumento).ToString("D2");
        string ambienteStr = ((int)ambiente).ToString();
        string serieStr = $"{serieEstablecimiento}{seriePuntoEmision}";
        string secuencialStr = secuencial.ToString("D9");
        string codigoStr = (codigoNumerico ?? GenerarCodigoAleatorio()).ToString("D8");
        const string tipoEmision = "1";

        string clave48 = $"{fechaStr}{tipoStr}{ruc}{ambienteStr}{serieStr}{secuencialStr}{codigoStr}{tipoEmision}";
        string verificador = CalcularDigitoVerificador(clave48).ToString();

        return $"{clave48}{verificador}";
    }

    private static int CalcularDigitoVerificador(string clave48)
    {
        int[] pesos = [2, 3, 4, 5, 6, 7];
        int suma = 0;

        for (int i = clave48.Length - 1; i >= 0; i--)
        {
            int peso = pesos[(clave48.Length - 1 - i) % pesos.Length];
            suma += int.Parse(clave48[i].ToString()) * peso;
        }

        int residuo = suma % 11;
        return residuo switch
        {
            0 => 0,
            1 => 1,
            _ => 11 - residuo
        };
    }

    private static int GenerarCodigoAleatorio() =>
        Random.Shared.Next(1, 99_999_999);
}
