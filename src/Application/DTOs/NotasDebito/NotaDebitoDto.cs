using Application.DTOs.Clientes;

namespace Application.DTOs.NotasDebito;

public class NotaDebitoDto
{
    public long Id { get; set; }
    public string? ClaveAcceso { get; set; }
    public string? NumeroAutorizacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? MotivoRechazo { get; set; }
    public DateTime FechaEmision { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Secuencial { get; set; } = string.Empty;
    public string NumeroCompleto => $"{Serie}-{Secuencial}";
    public long FacturaId { get; set; }
    public string NumDocModificado { get; set; } = string.Empty;
    public decimal TotalSinImpuestos { get; set; }
    public decimal TotalIva { get; set; }
    public decimal ValorTotal { get; set; }
    public ClienteDto? Cliente { get; set; }
    public List<MotivoDto> Motivos { get; set; } = new();
}
