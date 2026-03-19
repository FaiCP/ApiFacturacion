using API.Models;
using Application.Commands.Custodios;
using Application.DTOs.Custodios;
using Application.Queries.Custodios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Gestión de custodios de equipos</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustodiosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustodiosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene el listado paginado de custodios</summary>
    /// <param name="cantidad">Número de elementos por página</param>
    /// <param name="pagina">Número de página (base 0)</param>
    /// <param name="busqueda">Texto de búsqueda (nombre o cédula)</param>
    /// <returns>Lista paginada de custodios</returns>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<CustodioDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 0,
        [FromQuery] string busqueda = "")
    {
        var result = await _mediator.Send(new GetCustodiosQuery(cantidad, pagina, busqueda));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<CustodioDto>>.Ok(result));
    }

    /// <summary>Genera el reporte general de inventario con información de custodios (PDF)</summary>
    /// <returns>Archivo PDF del reporte</returns>
    [HttpGet("acta")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Acta()
    {
        var bytes = await _mediator.Send(new GenerarActaCustodiosPdfQuery());
        return File(bytes, "application/pdf", "acta_custodios.pdf");
    }

    /// <summary>Crea un nuevo custodio</summary>
    /// <param name="request">Datos del custodio</param>
    /// <returns>ID del custodio creado</returns>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear([FromBody] CustodioDto request)
    {
        var id = await _mediator.Send(new CreateCustodioCommand(
            request.NombreEmpleado,
            request.CargoEmpleado,
            request.CedulaEmpleado,
            request.IdDepartamento));
        return Ok(ApiResponse<long>.Ok(id));
    }

    /// <summary>Actualiza un custodio existente</summary>
    /// <param name="id">ID del custodio a actualizar</param>
    /// <param name="request">Nuevos datos del custodio</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] CustodioDto request)
    {
        var result = await _mediator.Send(new UpdateCustodioCommand(
            id,
            request.IdDepartamento,
            request.CedulaEmpleado,
            request.CargoEmpleado,
            request.NombreEmpleado));
        return Ok(ApiResponse<bool>.Ok(result));
    }

    /// <summary>Elimina (borrado lógico) custodios</summary>
    /// <param name="ids">Lista de IDs a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("Eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar([FromQuery] List<long> ids)
    {
        var result = await _mediator.Send(new DeleteCustodioCommand(ids));
        return Ok(ApiResponse<bool>.Ok(result));
    }
}
