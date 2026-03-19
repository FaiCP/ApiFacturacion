namespace Domain.Entities;

/// <summary>
/// Entidad de Hardware (gestion_hardware)
/// </summary>
public class Hardware : BaseEntity
{
    public string? IdEquipo { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public DateTime? FechaAdquisicion { get; set; }
    public string? Estado { get; set; }
    public string? Ubicacion { get; set; }
    public string? CodigoCne { get; set; }
    public string? NombreDispositivo { get; set; }
    public long? IdSuministro { get; set; }
    public string? Observacion { get; set; }
    public decimal? Valor { get; set; }

    // Navegación
    public virtual ICollection<CaracteristicaComputadora> Caracteristicas { get; set; } = new List<CaracteristicaComputadora>();
    public virtual ICollection<ControlActivo> ControlesActivos { get; set; } = new List<ControlActivo>();
    public virtual ICollection<GestionActivo> GestionActivos { get; set; } = new List<GestionActivo>();
    public virtual ICollection<Suministro> Suministros { get; set; } = new List<Suministro>();
}
