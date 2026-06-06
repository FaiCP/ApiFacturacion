using System.ComponentModel;

namespace Domain.Entities;

/// <summary>
/// Tipo de suministro remanufacturado
/// </summary>
public enum TipoSuministro
{
    [Description("Toner")]
    Toner = 0,

    [Description("Cartucho")]
    Cartucho = 1,

    [Description("Cinta")]
    Cinta = 2,

    [Description("Tambor")]
    Tambor = 3,

    [Description("Componente")]
    Componente = 4
}