using Domain.Entities;

namespace Domain.Interfaces;

public interface IXmlRetencionService
{
    string GenerarXml(Retencion retencion);
}
