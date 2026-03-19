using API.Models;
using Application.Commands.GestionActivos;
using Application.DTOs.GestionActivos;
using Application.Queries.GestionActivos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Gestión de activos asignados a custodios</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GestionActivosController : ControllerBase
{
    private readonly IMediator _mediator;

    public GestionActivosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene el listado paginado de activos asignados</summary>
    /// <param name="cantidad">Número de elementos por página</param>
    /// <param name="pagina">Número de página (base 0)</param>
    /// <param name="busqueda">Texto de búsqueda (activo, marca, custodio, etc.)</param>
    /// <returns>Lista paginada de activos asignados</returns>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<GestionActivoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 0,
        [FromQuery] string busqueda = "")
    {
        var result = await _mediator.Send(new GetGestionActivosQuery(cantidad, pagina, busqueda));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<GestionActivoDto>>.Ok(result));
    }

    /// <summary>Genera el acta de entrega-recepción de equipos (PDF)</summary>
    /// <param name="ids">Lista de IDs de asignaciones activas</param>
    /// <returns>Archivo PDF del acta</returns>
    [HttpGet("acta")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Acta([FromQuery] List<long> ids)
    {
        var bytes = await _mediator.Send(new GenerarActaGestionActivosPdfQuery(ids));
        return File(bytes, "application/pdf", "acta_gestion_activos.pdf");
    }

    /// <summary>Genera el acta de devolución de equipos (PDF)</summary>
    /// <param name="ids">Lista de IDs de asignaciones devueltas</param>
    /// <returns>Archivo PDF del acta de devolución</returns>
    [HttpGet("devolucion")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Devolucion([FromQuery] List<long> ids)
    {
        var bytes = await _mediator.Send(new GenerarDevolucionGestionActivosPdfQuery(ids));
        return File(bytes, "application/pdf", "devolucion_gestion_activos.pdf");
    }

    /// <summary>Asigna uno o más equipos a un custodio</summary>
    /// <param name="request">Lista de asignaciones de equipos</param>
    /// <returns>Lista de IDs creados</returns>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<List<long?>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear([FromBody] List<AsignacionEquipoDto> request)
    {
        var ids = await _mediator.Send(new CreateGestionActivosCommand(request));
        return Ok(ApiResponse<List<long?>>.Ok(ids));
    }

    /// <summary>Registra la devolución de un activo (establece fecha de devolución)</summary>
    /// <param name="id">ID de la asignación a actualizar</param>
    /// <param name="request">Datos de la devolución</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] GestionActivoDto request)
    {
        var result = await _mediator.Send(new UpdateGestionActivoCommand(
            id,
            request.IdEquipo,
            request.IdCustodio,
            request.FechaAsignacion,
            request.FechaDevolucion));
        return Ok(ApiResponse<bool>.Ok(result));
    }

    /// <summary>Elimina (borrado lógico) asignaciones de activos</summary>
    /// <param name="ids">Lista de IDs a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("Eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar([FromQuery] List<long> ids)
    {
        var result = await _mediator.Send(new DeleteGestionActivoCommand(ids));
        return Ok(ApiResponse<bool>.Ok(result));
    }
}
