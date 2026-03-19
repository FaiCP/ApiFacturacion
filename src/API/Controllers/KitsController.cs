using API.Models;
using Application.Commands.Kits;
using Application.DTOs.Kits;
using Application.Queries.Kits;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Gestión de kits e insumos</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public KitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene el listado paginado de kits</summary>
    /// <param name="cantidad">Número de elementos por página</param>
    /// <param name="pagina">Número de página (base 0)</param>
    /// <param name="busqueda">Texto de búsqueda (insumo, marca, modelo, serie)</param>
    /// <returns>Lista paginada de kits</returns>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<KitDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 0,
        [FromQuery] string busqueda = "")
    {
        var result = await _mediator.Send(new GetKitsQuery(cantidad, pagina, busqueda));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<KitDto>>.Ok(result));
    }

    /// <summary>Genera el informe de inventario de kits (PDF)</summary>
    /// <returns>Archivo PDF del informe</returns>
    [HttpGet("GenerarActa")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerarActa()
    {
        var bytes = await _mediator.Send(new GenerarActaKitsPdfQuery());
        return File(bytes, "application/pdf", "acta_kits.pdf");
    }

    /// <summary>Crea un nuevo kit o insumo</summary>
    /// <param name="request">Datos del kit</param>
    /// <returns>ID del kit creado</returns>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear([FromBody] KitDto request)
    {
        var id = await _mediator.Send(new CreateKitCommand(
            request.Insumo,
            request.Cantidad,
            request.Estado,
            request.Observacion,
            request.Marca,
            request.Serie,
            request.Modelo));
        return Ok(ApiResponse<long>.Ok(id));
    }

    /// <summary>Actualiza un kit existente</summary>
    /// <param name="id">ID del kit a actualizar</param>
    /// <param name="request">Nuevos datos del kit</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] KitDto request)
    {
        var result = await _mediator.Send(new UpdateKitCommand(
            id,
            request.Insumo,
            request.Cantidad,
            request.Estado,
            request.Observacion,
            request.Marca,
            request.Serie,
            request.Modelo));
        return Ok(ApiResponse<bool>.Ok(result));
    }

    /// <summary>Elimina (borrado lógico) kits</summary>
    /// <param name="ids">Lista de IDs a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("Eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar([FromQuery] List<long> ids)
    {
        var result = await _mediator.Send(new DeleteKitCommand(ids));
        return Ok(ApiResponse<bool>.Ok(result));
    }
}
