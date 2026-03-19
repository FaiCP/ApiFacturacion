using API.Models;
using Application.Commands.Hardware;
using Application.DTOs.Hardware;
using Application.Queries.Hardware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Gestión de hardware e inventario de equipos</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HardwareController : ControllerBase
{
    private readonly IMediator _mediator;

    public HardwareController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene el listado paginado de hardware</summary>
    /// <param name="cantidad">Número de elementos por página</param>
    /// <param name="pagina">Número de página (base 0)</param>
    /// <param name="busqueda">Texto de búsqueda (activo, marca, modelo, etc.)</param>
    /// <returns>Lista paginada de hardware</returns>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<HardwareDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 0,
        [FromQuery] string busqueda = "")
    {
        var result = await _mediator.Send(new GetHardwareQuery(cantidad, pagina, busqueda));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<HardwareDto>>.Ok(result));
    }

    /// <summary>Genera el informe de inventario de hardware (PDF)</summary>
    /// <returns>Archivo PDF del informe</returns>
    [HttpGet("GenerarActa")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerarActa()
    {
        var bytes = await _mediator.Send(new GenerarActaHardwarePdfQuery());
        return File(bytes, "application/pdf", "acta_hardware.pdf");
    }

    /// <summary>Genera el informe de inventario de hardware (Excel)</summary>
    /// <returns>Archivo Excel del informe</returns>
    [HttpGet("GenerarActaExcel")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerarActaExcel()
    {
        var bytes = await _mediator.Send(new GenerarActaHardwareExcelQuery());
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "acta_hardware.xlsx");
    }

    /// <summary>Crea un nuevo equipo de hardware junto a sus características técnicas si aplica</summary>
    /// <param name="request">Datos del hardware y características opcionales</param>
    /// <returns>ID del hardware creado</returns>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear([FromBody] HardwareCreateDto request)
    {
        var id = await _mediator.Send(new CreateHardwareCommand(
            request.Ubicacion,
            request.Descripcion,
            request.NombreDispositivo,
            request.Marca,
            request.Modelo,
            request.CodigoCne,
            request.IdEquipo,
            request.Estado,
            request.Ram,
            request.Rom,
            request.Procesador,
            request.Valor));
        return Ok(ApiResponse<long>.Ok(id));
    }

    /// <summary>Actualiza un equipo de hardware existente</summary>
    /// <param name="id">ID del hardware a actualizar</param>
    /// <param name="request">Nuevos datos del hardware</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] HardwareDto request)
    {
        var result = await _mediator.Send(new UpdateHardwareCommand(
            id,
            request.IdEquipo,
            request.Descripcion,
            request.Marca,
            request.Modelo,
            request.Estado,
            request.Ubicacion,
            request.CodigoCne,
            request.NombreDispositivo,
            request.Valor));
        return Ok(ApiResponse<bool>.Ok(result));
    }

    /// <summary>Elimina (borrado lógico) equipos de hardware</summary>
    /// <param name="ids">Lista de IDs a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("Eliminar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Eliminar([FromQuery] List<long> ids)
    {
        var result = await _mediator.Send(new DeleteHardwareCommand(ids));
        return Ok(ApiResponse<bool>.Ok(result));
    }
}
