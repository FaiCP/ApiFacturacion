using API.Models;
using Application.DTOs.Reportes;
using Application.Queries.Reportes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Reportes estadísticos del inventario</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene el conteo total de inventario agrupado por tipo de dispositivo</summary>
    /// <returns>Lista con nombre del dispositivo y cantidad total</returns>
    [HttpGet("InventarioTotal")]
    [ProducesResponseType(typeof(ApiResponse<List<ReporteInventarioDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InventarioTotal()
    {
        var result = await _mediator.Send(new GetInventarioTotalQuery());
        return Ok(ApiResponse<List<ReporteInventarioDto>>.Ok(result));
    }

    /// <summary>Obtiene el total de préstamos y devoluciones agrupados por mes</summary>
    /// <returns>Lista con mes, total de préstamos y total de devoluciones</returns>
    [HttpGet("PrestamosPorMes")]
    [ProducesResponseType(typeof(ApiResponse<List<ReportePrestamosDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PrestamosPorMes()
    {
        var result = await _mediator.Send(new GetPrestamosPorMesQuery());
        return Ok(ApiResponse<List<ReportePrestamosDto>>.Ok(result));
    }

    /// <summary>Obtiene los equipos prestados actualmente agrupados por tipo de dispositivo y mes</summary>
    /// <returns>Lista con nombre del dispositivo, total prestados, mes y año</returns>
    [HttpGet("EquiposPrestadosPorTipo")]
    [ProducesResponseType(typeof(ApiResponse<List<ReportePrestadosDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EquiposPrestadosPorTipo()
    {
        var result = await _mediator.Send(new GetEquiposPrestadosPorTipoQuery());
        return Ok(ApiResponse<List<ReportePrestadosDto>>.Ok(result));
    }
}
