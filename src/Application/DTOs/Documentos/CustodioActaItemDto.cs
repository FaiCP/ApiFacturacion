namespace Application.DTOs.Documentos;

public record CustodioActaItemDto(
    string IdEquipo,
    DateTime Fecha,
    string NombreCustodio,
    string Marca,
    string Modelo,
    string CodigoCne,
    decimal? Valor,
    string Estado,
    string NombreDispositivo
);
