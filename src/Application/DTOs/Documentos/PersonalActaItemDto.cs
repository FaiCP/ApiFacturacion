namespace Application.DTOs.Documentos;

public record PersonalActaItemDto(
    string Nombre,
    string Cedula,
    string Cargo,
    DateTime? Fecha,
    string Email,
    string? TempPass
);
