namespace Application.DTOs.Hardware;

public class CaracteristicaComputadoraDto
{
    public long Id { get; set; }
    public string IdEquipo { get; set; } = string.Empty;
    public string Ram { get; set; } = string.Empty;
    public string Rom { get; set; } = string.Empty;
    public string Procesador { get; set; } = string.Empty;
}
