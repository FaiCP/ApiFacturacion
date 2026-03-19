using API.Models;
using Application.DTOs.HistorialPrestamos;
using Application.Queries.HistorialPrestamos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Historial de préstamos de equipos por custodio</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistorialPrestamosController : ControllerBase
{
    private readonly IMediator _mediator;

    public HistorialPrestamosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene el historial paginado de préstamos filtrado por custodio</summary>
    /// <param name="cantidad">Número de elementos por página</param>
    /// <param name="pagina">Número de página (base 0)</param>
    /// <param name="busqueda">Texto de búsqueda (activo, dispositivo, código CNE)</param>
    /// <param name="idCustodio">ID del custodio para filtrar (0 = todos)</param>
    /// <returns>Lista paginada del historial de préstamos</returns>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<HistorialPrestamoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 0,
        [FromQuery] string busqueda = "",
        [FromQuery] int idCustodio = 0)
    {
        var result = await _mediator.Send(new GetHistorialPrestamosQuery(cantidad, pagina, busqueda, idCustodio));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<HistorialPrestamoDto>>.Ok(result));
    }

    /// <summary>Genera el reporte de bienes por custodio (PDF)</summary>
    /// <param name="ids">Lista de IDs del historial</param>
    /// <returns>Archivo PDF del reporte</returns>
    [HttpGet("GenerarActa")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerarActa([FromQuery] List<long> ids)
    {
        var bytes = await _mediator.Send(new GenerarActaHistorialPrestamosPdfQuery(ids));
        return File(bytes, "application/pdf", "acta_historial_prestamos.pdf");
    }
}
