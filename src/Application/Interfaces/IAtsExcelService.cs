using Domain.Entities;

namespace Application.Interfaces;

public interface IAtsExcelService
{
    byte[] GenerarAts(List<Factura> facturas, List<Retencion> retenciones, int anio, int mes);
}
