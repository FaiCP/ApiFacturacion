namespace Application.DTOs.Documentos;

public record KitActaItemDto(
    string Insumo,
    string Marca,
    string Modelo,
    string Serie,
    string Cantidad,
    string Estado,
    string Observacion
);
