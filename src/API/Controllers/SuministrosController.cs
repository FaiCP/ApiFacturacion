using API.Models;
using Application.Commands.Suministros;
using Application.DTOs.Suministros;
using Application.Queries.Suministros;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Gestión de suministros remanufacturados</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuministrosController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuministrosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene el listado paginado de suministros</summary>
    /// <param name="cantidad">Número de elementos por página</param>
    /// <param name="pagina">Número de página (base 0)</param>
    /// <param name="busqueda">Texto de búsqueda opcional</param>
    /// <returns>Lista paginada de suministros</returns>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<SuministroDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 0,
        [FromQuery] string busqueda = "")
    {
        var result = await _mediator.Send(new GetSuministrosQuery(cantidad, pagina, busqueda));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<SuministroDto>>.Ok(result));
    }

    /// <summary>Crea un nuevo suministro remanufacturado</summary>
    /// <param name="request">Datos del suministro</param>
    /// <returns>ID del suministro creado</returns>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear([FromBody] SuministroDto request)
    {
        var id = await _mediator.Send(new CreateSuministroCommand(
            request.IdEquipo,
            request.TipoSuministro,
            request.IdEquipoAsignado,
            request.FechaRetiro));
        return Ok(ApiResponse<long>.Ok(id));
    }

    /// <summary>Actualiza un suministro existente</summary>
    /// <param name="id">ID del suministro a actualizar</param>
    /// <param name="request">Nuevos datos del suministro</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] SuministroDto request)
    {
        var result = await _mediator.Send(new UpdateSuministroCommand(
            id,
            request.IdEquipo,
            request.TipoSuministro,
            request.IdEquipoAsignado,
            request.FechaRetiro));
        return Ok(ApiResponse<bool>.Ok(result));
    }

    /// <summary>Elimina (borrado lógico) suministros</summary>
    /// <param name="ids">Lista de IDs a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("Eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar([FromQuery] List<long> ids)
    {
        var result = await _mediator.Send(new DeleteSuministroCommand(ids));
        return Ok(ApiResponse<bool>.Ok(result));
    }
}
