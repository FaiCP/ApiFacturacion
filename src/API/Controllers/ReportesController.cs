using API.Models;
using Application.DTOs.Reportes;
using Application.Queries.Reportes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Reportes de facturación electrónica</summary>
[ApiController]
[Route("api/v1/reportes")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ── REPORTES FACTURACIÓN ELECTRÓNICA ─────────────────────────────────────

    /// <summary>Facturas emitidas agrupadas por mes (solo AUTORIZADA/ENVIADA)</summary>
    [HttpGet("facturas-por-mes")]
    public async Task<IActionResult> FacturasPorMes([FromQuery] int? anio = null)
    {
        var result = await _mediator.Send(new GetFacturasPorMesQuery(anio));
        return Ok(ApiResponse<List<FacturasPorMesDto>>.Ok(result));
    }

    /// <summary>IVA desglosado por mes (base 0%, base 15%, total IVA)</summary>
    [HttpGet("iva-por-mes")]
    public async Task<IActionResult> IvaPorMes([FromQuery] int? anio = null)
    {
        var result = await _mediator.Send(new GetIvaPorMesQuery(anio));
        return Ok(ApiResponse<List<IvaPorMesDto>>.Ok(result));
    }

    /// <summary>Conteo de facturas por estado SRI</summary>
    [HttpGet("facturas-por-estado")]
    public async Task<IActionResult> FacturasPorEstado()
    {
        var result = await _mediator.Send(new GetFacturasPorEstadoQuery());
        return Ok(ApiResponse<List<FacturasPorEstadoDto>>.Ok(result));
    }

    /// <summary>Top clientes por monto facturado (solo AUTORIZADAS)</summary>
    [HttpGet("top-clientes")]
    public async Task<IActionResult> TopClientes([FromQuery] int top = 10, [FromQuery] int? anio = null)
    {
        var result = await _mediator.Send(new GetTopClientesQuery(top, anio));
        return Ok(ApiResponse<List<TopClienteDto>>.Ok(result));
    }

    /// <summary>Descarga ATS Excel para declaración SRI mensual</summary>
    [HttpGet("ats/{anio:int}/{mes:int}")]
    public async Task<IActionResult> DescargarAts(int anio, int mes)
    {
        if (mes < 1 || mes > 12) return BadRequest(ApiResponse<string>.Fail("Mes debe estar entre 1 y 12."));
        var excel = await _mediator.Send(new GetAtsSRIQuery(anio, mes));
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"ATS_{anio}_{mes:D2}.xlsx");
    }
}
