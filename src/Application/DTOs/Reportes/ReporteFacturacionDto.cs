namespace Application.DTOs.Reportes;

public record FacturasPorMesDto(
    int Anio, int Mes, string MesNombre,
    int TotalFacturas, decimal TotalMonto, decimal TotalIva, decimal TotalSinImpuestos);

public record IvaPorMesDto(
    int Anio, int Mes, string MesNombre,
    decimal BaseImponible0, decimal BaseImponible15,
    decimal TotalIva, decimal ImporteTotal);

public record FacturasPorEstadoDto(string Estado, int Cantidad, decimal TotalMonto);

public record TopClienteDto(
    string Identificacion, string RazonSocial,
    int TotalFacturas, decimal TotalFacturado);
