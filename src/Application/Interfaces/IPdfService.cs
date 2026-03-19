using Application.DTOs.Documentos;

namespace Application.Interfaces;

public interface IPdfService
{
    byte[] GenerarActaHardware(List<HardwareActaItemDto> items);
    byte[] GenerarActaCustodios(List<CustodioActaItemDto> items);
    byte[] GenerarActaPersonal(PersonalActaItemDto persona);
    Task<byte[]> GenerarReportePersonalAsync(List<ReportePersonalItemDto> items);
    byte[] GenerarActaKits(List<KitActaItemDto> items);
    byte[] GenerarActaGestionActivos(GestionActivoActaItemDto primerEquipo, List<GestionActivoActaItemDto> items);
    byte[] GenerarDevolucionGestionActivos(GestionActivoActaItemDto primerEquipo, List<GestionActivoActaItemDto> items);
    byte[] GenerarActaHistorialPrestamos(HistorialActaItemDto primerEquipo, List<HistorialActaItemDto> items);
}
