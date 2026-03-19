using API.Models;
using Application.Commands.Personal;
using Application.DTOs.Personal;
using Application.Queries.Personal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Gestión del personal de la organización</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonalController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene el listado paginado de personal</summary>
    /// <param name="cantidad">Número de elementos por página</param>
    /// <param name="pagina">Número de página (base 0)</param>
    /// <param name="busqueda">Texto de búsqueda (nombre o cédula)</param>
    /// <returns>Lista paginada de personal</returns>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<PersonalDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 0,
        [FromQuery] string busqueda = "")
    {
        var result = await _mediator.Send(new GetPersonalQuery(cantidad, pagina, busqueda));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<PersonalDto>>.Ok(result));
    }

    /// <summary>Genera el acta de entrega-recepción de credenciales para el personal indicado (PDF)</summary>
    /// <param name="ids">Lista de IDs de personal</param>
    /// <returns>Archivo PDF del acta</returns>
    [HttpGet("GenerarActa")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerarActa([FromQuery] List<long> ids)
    {
        var bytes = await _mediator.Send(new GenerarActaPersonalPdfQuery(ids));
        return File(bytes, "application/pdf", "acta_personal.pdf");
    }

    /// <summary>Genera el reporte de entrega-recepción de equipos y sistemas (PDF)</summary>
    /// <returns>Archivo PDF del reporte</returns>
    [HttpGet("GenerarReporte")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerarReporte()
    {
        var bytes = await _mediator.Send(new GenerarReportePersonalPdfQuery());
        return File(bytes, "application/pdf", "reporte_personal.pdf");
    }

    /// <summary>Genera el reporte de entrega-recepción de equipos y sistemas (Excel)</summary>
    /// <returns>Archivo Excel del reporte</returns>
    [HttpGet("GenerarReporteExel")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerarReporteExel()
    {
        var bytes = await _mediator.Send(new GenerarReportePersonalExcelQuery());
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reporte_personal.xlsx");
    }

    /// <summary>Crea un nuevo registro de personal</summary>
    /// <param name="request">Datos del personal</param>
    /// <returns>ID del personal creado</returns>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear([FromBody] PersonalDto request)
    {
        var id = await _mediator.Send(new CreatePersonalCommand(
            request.Nombre,
            request.Cedula,
            request.Cargo,
            request.Email,
            request.TempPass,
            request.Fecha));
        return Ok(ApiResponse<long>.Ok(id));
    }

    /// <summary>Actualiza un registro de personal existente</summary>
    /// <param name="id">ID del personal a actualizar</param>
    /// <param name="request">Nuevos datos del personal</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] PersonalDto request)
    {
        var result = await _mediator.Send(new UpdatePersonalCommand(
            id,
            request.Nombre,
            request.Cedula,
            request.Cargo,
            request.Email,
            request.TempPass,
            request.Fecha));
        return Ok(ApiResponse<bool>.Ok(result));
    }

    /// <summary>Elimina (borrado lógico) registros de personal</summary>
    /// <param name="ids">Lista de IDs a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("Eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar([FromQuery] List<long> ids)
    {
        var result = await _mediator.Send(new DeletePersonalCommand(ids));
        return Ok(ApiResponse<bool>.Ok(result));
    }
}
