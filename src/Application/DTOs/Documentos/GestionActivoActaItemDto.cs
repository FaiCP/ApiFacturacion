namespace Application.DTOs.Documentos;

public record GestionActivoActaItemDto(
    string IdEquipo,
    DateTime Fecha,
    string NombreDispositivo,
    string Marca,
    string Modelo,
    string CodigoCne,
    string Estado,
    string NombreCustodio,
    string Cargo,
    string Departamento
);
