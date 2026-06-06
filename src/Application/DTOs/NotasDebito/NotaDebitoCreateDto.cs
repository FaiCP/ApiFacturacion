namespace Application.DTOs.NotasDebito;

public class NotaDebitoCreateDto
{
    public long FacturaId { get; set; }
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public List<MotivoDto> Motivos { get; set; } = new();
}

public class MotivoDto
{
    public string Razon { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
