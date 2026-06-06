namespace Domain.Entities;

public class MotivoNotaDebito : BaseEntity
{
    public long NotaDebitoId { get; set; }
    public string Razon { get; set; } = string.Empty;
    public decimal Valor { get; set; }

    public virtual NotaDebito? NotaDebito { get; set; }
}
