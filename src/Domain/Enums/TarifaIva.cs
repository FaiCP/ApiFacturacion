namespace Domain.Enums;

public enum TarifaIva
{
    Cero = 0,       // 0%  — código SRI "0"
    Quince = 15,    // 15% — código SRI "2" (vigente desde 2024)
    Exento = -1,    // Exento de IVA — código SRI "6"
    NoObjeto = -2   // No objeto de IVA — código SRI "7"
}
