using Domain.Entities;

namespace Domain.Interfaces;

public interface IXmlFacturaService
{
    string GenerarXml(Factura factura);
}
