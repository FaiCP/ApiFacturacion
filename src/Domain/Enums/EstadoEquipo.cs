using System.ComponentModel;

namespace Domain.Entities;

/// <summary>
/// Estado del equipo de hardware
/// </summary>
public enum EstadoEquipo
{
    [Description("Activo")]
    Activo = 0,

    [Description("Inactivo")]
    Inactivo = 1,

    [Description("En Mantenimiento")]
    EnMantenimiento = 2,

    [Description("Dado de Baja")]
    DadoDeBaja = 3
}