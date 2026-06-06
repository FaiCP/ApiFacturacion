using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Productos;

public record CreateProductoCommand(
    string CodigoPrincipal,
    string? CodigoAuxiliar,
    string Descripcion,
    decimal PrecioUnitario,
    TarifaIva TarifaIva,
    bool EsServicio) : IRequest<long>;

public class CreateProductoCommandHandler : IRequestHandler<CreateProductoCommand, long>
{
    private readonly IProductoRepository _productoRepository;

    public CreateProductoCommandHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<long> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = new Domain.Entities.Producto
        {
            CodigoPrincipal = request.CodigoPrincipal,
            CodigoAuxiliar = request.CodigoAuxiliar,
            Descripcion = request.Descripcion,
            PrecioUnitario = request.PrecioUnitario,
            TarifaIva = request.TarifaIva,
            EsServicio = request.EsServicio,
            Activo = true,
            Borrado = false
        };

        var result = await _productoRepository.AddAsync(producto);
        return result.Id;
    }
}
