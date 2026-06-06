namespace Domain.Validators;

/// <summary>
/// Algoritmo módulo 10 — cédula ecuatoriana 10 dígitos
/// </summary>
public static class CedulaValidator
{
    public static bool EsValida(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula) || cedula.Length != 10)
            return false;

        if (!cedula.All(char.IsDigit))
            return false;

        int provincia = int.Parse(cedula[..2]);
        if (provincia < 1 || provincia > 24)
            return false;

        int[] coeficientes = [2, 1, 2, 1, 2, 1, 2, 1, 2];
        int suma = 0;

        for (int i = 0; i < 9; i++)
        {
            int digito = int.Parse(cedula[i].ToString());
            int resultado = digito * coeficientes[i];
            if (resultado >= 10)
                resultado -= 9;
            suma += resultado;
        }

        int digitoVerificador = (10 - (suma % 10)) % 10;
        return digitoVerificador == int.Parse(cedula[9].ToString());
    }
}
