using System.ComponentModel;

namespace Domain.Entities;

/// <summary>
/// Estado del kit de insumos
/// </summary>
public enum EstadoKit
{
    [Description("Disponible")]
    Disponible = 0,

    [Description("Asignado")]
    Asignado = 1,

    [Description("En Mantenimiento")]
    EnMantenimiento = 2,

    [Description("Dado de Baja")]
    DadoDeBaja = 3
}