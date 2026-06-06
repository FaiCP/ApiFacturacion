namespace Domain.Validators;

/// <summary>
/// Valida RUC ecuatoriano (13 dígitos).
/// Tres tipos: persona natural, sociedad privada, entidad pública.
/// </summary>
public static class RucValidator
{
    public static bool EsValido(string ruc)
    {
        if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 13)
            return false;

        if (!ruc.All(char.IsDigit))
            return false;

        int provincia = int.Parse(ruc[..2]);
        if (provincia < 1 || provincia > 24)
            return false;

        int tercerDigito = int.Parse(ruc[2].ToString());

        return tercerDigito switch
        {
            < 6 => ValidarPersonaNatural(ruc),
            6 => ValidarEntidadPublica(ruc),
            9 => ValidarSociedadPrivada(ruc),
            _ => false
        };
    }

    // Persona natural: primeros 10 dígitos = cédula válida, últimos 3 != "000"
    private static bool ValidarPersonaNatural(string ruc)
    {
        if (ruc[10..] == "000")
            return false;
        return CedulaValidator.EsValida(ruc[..10]);
    }

    // Sociedad privada: tercer dígito = 9, módulo 11 sobre primeros 9
    private static bool ValidarSociedadPrivada(string ruc)
    {
        if (ruc[10..] == "000")
            return false;

        int[] coeficientes = [4, 3, 2, 7, 6, 5, 4, 3, 2];
        return ValidarModulo11(ruc, coeficientes, verificadorPos: 9);
    }

    // Entidad pública: tercer dígito = 6, módulo 11 sobre primeros 8
    private static bool ValidarEntidadPublica(string ruc)
    {
        if (ruc[9..] == "0001")
            return false;

        int[] coeficientes = [3, 2, 7, 6, 5, 4, 3, 2];
        return ValidarModulo11(ruc, coeficientes, verificadorPos: 8);
    }

    private static bool ValidarModulo11(string ruc, int[] coeficientes, int verificadorPos)
    {
        int suma = 0;
        for (int i = 0; i < coeficientes.Length; i++)
            suma += int.Parse(ruc[i].ToString()) * coeficientes[i];

        int residuo = suma % 11;
        int verificador = residuo == 0 ? 0 : 11 - residuo;

        return verificador == int.Parse(ruc[verificadorPos].ToString());
    }
}
