namespace Domain.Interfaces;

public interface IFirmaDigitalService
{
    Task<string> FirmarXmlAsync(string xml, string certificadoBase64, string password);
}
