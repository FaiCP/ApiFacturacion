using API.Models;
using Application.Commands.Productos;
using Application.DTOs.Productos;
using Application.Queries.Productos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Catálogo de productos y servicios</summary>
[ApiController]
[Route("api/v1/productos")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listado paginado de productos</summary>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<ProductoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 1,
        [FromQuery] string busqueda = "")
    {
        var result = await _mediator.Send(new GetProductosQuery(cantidad, pagina, busqueda));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<ProductoDto>>.Ok(result));
    }

    /// <summary>Crea un producto o servicio</summary>
    [HttpPost("Crear")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear([FromBody] ProductoCreateDto dto)
    {
        var id = await _mediator.Send(new CreateProductoCommand(
            dto.CodigoPrincipal, dto.CodigoAuxiliar, dto.Descripcion,
            dto.PrecioUnitario, dto.TarifaIva, dto.EsServicio));

        return StatusCode(StatusCodes.Status201Created, ApiResponse<long>.Ok(id, "Producto creado correctamente."));
    }

    /// <summary>Actualiza un producto</summary>
    [HttpPut("Actualizar/{id:long}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ProductoCreateDto dto)
    {
        var result = await _mediator.Send(new UpdateProductoCommand(
            id, dto.Descripcion, dto.PrecioUnitario, dto.TarifaIva, dto.EsServicio, true));

        return Ok(ApiResponse<bool>.Ok(result, "Producto actualizado correctamente."));
    }
}
