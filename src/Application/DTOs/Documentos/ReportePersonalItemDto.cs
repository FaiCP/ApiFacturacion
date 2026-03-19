namespace Application.DTOs.Documentos;

public record ReportePersonalItemDto(
    DateTime? Fecha,
    string Entrega,
    string Recibe,
    string? EquiposE,
    string? EquiposP,
    string Observacion
);
