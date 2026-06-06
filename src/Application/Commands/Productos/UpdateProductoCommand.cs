using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Productos;

public record UpdateProductoCommand(
    long Id,
    string Descripcion,
    decimal PrecioUnitario,
    TarifaIva TarifaIva,
    bool EsServicio,
    bool Activo) : IRequest<bool>;

public class UpdateProductoCommandHandler : IRequestHandler<UpdateProductoCommand, bool>
{
    private readonly IProductoRepository _productoRepository;

    public UpdateProductoCommandHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<bool> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Producto con Id {request.Id} no encontrado.");

        producto.Descripcion = request.Descripcion;
        producto.PrecioUnitario = request.PrecioUnitario;
        producto.TarifaIva = request.TarifaIva;
        producto.EsServicio = request.EsServicio;
        producto.Activo = request.Activo;
        producto.UpdatedAt = DateTime.UtcNow;

        await _productoRepository.UpdateAsync(producto);
        return true;
    }
}
