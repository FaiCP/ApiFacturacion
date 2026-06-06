using Domain.Entities;

namespace Domain.Interfaces;

public interface IXmlNotaCreditoService
{
    string GenerarXml(NotaCredito notaCredito);
}
