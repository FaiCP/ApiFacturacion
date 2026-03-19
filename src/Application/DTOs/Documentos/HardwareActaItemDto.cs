namespace Application.DTOs.Documentos;

public record HardwareActaItemDto(
    string IdEquipo,
    string NombreCustodio,
    string NombreDispositivo,
    string Marca,
    string Modelo,
    string CodigoCne,
    string? Ram,
    string? Rom,
    string? Procesador,
    decimal? Valor,
    string Estado
);
