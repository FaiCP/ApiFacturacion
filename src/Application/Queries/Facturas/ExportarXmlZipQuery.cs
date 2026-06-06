using Domain.Interfaces;
using MediatR;
using System.IO.Compression;
using System.Text;

namespace Application.Queries.Facturas;

public record ExportarXmlZipQuery(DateTime Desde, DateTime Hasta) : IRequest<byte[]>;

public class ExportarXmlZipQueryHandler : IRequestHandler<ExportarXmlZipQuery, byte[]>
{
    private readonly IReporteFacturacionRepository _repo;
    public ExportarXmlZipQueryHandler(IReporteFacturacionRepository repo) => _repo = repo;

    public async Task<byte[]> Handle(ExportarXmlZipQuery request, CancellationToken ct)
    {
        var facturas = await _repo.GetFacturasConXmlAsync(request.Desde, request.Hasta);

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var f in facturas)
            {
                var nombre = $"FACT_{f.Serie}_{f.Secuencial}_{f.ClaveAcceso?[..8]}.xml";
                var entry = zip.CreateEntry(nombre, CompressionLevel.Fastest);
                using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
                await writer.WriteAsync(f.XmlFirmado);
            }
        }

        ms.Position = 0;
        return ms.ToArray();
    }
}
