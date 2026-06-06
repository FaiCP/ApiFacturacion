using Domain.Entities;

namespace Domain.Interfaces;

public interface IXmlNotaDebitoService
{
    string GenerarXml(NotaDebito notaDebito);
}
