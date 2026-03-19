namespace Application.DTOs.Documentos;

public record HistorialActaItemDto(
    string IdEquipo,
    string NombreDispositivo,
    string CodigoCne,
    string Modelo,
    string Marca,
    decimal? Valor,
    string Estado,
    string Descripcion,
    string NombreCustodio,
    string Cargo,
    string Departamento
);
