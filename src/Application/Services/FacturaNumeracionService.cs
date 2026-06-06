using Domain.Interfaces;

namespace Application.Services;

public class FacturaNumeracionService
{
    private readonly IFacturaRepository _facturaRepository;

    public FacturaNumeracionService(IFacturaRepository facturaRepository)
    {
        _facturaRepository = facturaRepository;
    }

    public async Task<(string Serie, string Secuencial)> GenerarSiguienteAsync(long emisorId, string serieEstablecimiento, string seriePuntoEmision)
    {
        var serie = $"{serieEstablecimiento}-{seriePuntoEmision}";
        var ultimo = await _facturaRepository.GetUltimoSecuencialAsync(emisorId, serie);
        var siguiente = (ultimo + 1).ToString("D9");
        return (serie, siguiente);
    }
}
