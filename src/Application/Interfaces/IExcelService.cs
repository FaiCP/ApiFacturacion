using Application.DTOs.Documentos;

namespace Application.Interfaces;

public interface IExcelService
{
    byte[] GenerarActaHardware(List<HardwareActaItemDto> items);
    byte[] GenerarReportePersonal(List<ReportePersonalItemDto> items);
}
