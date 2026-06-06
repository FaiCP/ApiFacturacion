using API.Models;
using Application.Commands.Facturas;
using Application.DTOs.Facturas;
using Application.Queries.Facturas;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmitirResult = Application.Commands.Facturas.EmitirFacturaResult;

namespace API.Controllers;

/// <summary>Gestión de facturas electrónicas</summary>
[ApiController]
[Route("api/v1/facturas")]
[Authorize]
public class FacturasController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacturasController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listado paginado de facturas</summary>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<FacturaResumenDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 1,
        [FromQuery] EstadoSRI? estado = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null,
        [FromQuery] long? clienteId = null)
    {
        var result = await _mediator.Send(new GetFacturasQuery(cantidad, pagina, estado, desde, hasta, clienteId));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<FacturaResumenDto>>.Ok(result));
    }

    /// <summary>Detalle completo de una factura</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<FacturaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetFacturaByIdQuery(id));
        return Ok(ApiResponse<FacturaDto>.Ok(result));
    }

    /// <summary>Crea una factura en estado BORRADOR</summary>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear([FromBody] FacturaCreateDto dto)
    {
        var id = await _mediator.Send(new CreateFacturaCommand(dto.ClienteId, dto.FechaEmision, dto.Detalles));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<long>.Ok(id, "Factura creada en estado BORRADOR."));
    }

    /// <summary>Anula una factura (solo BORRADOR o RECHAZADA)</summary>
    [HttpPost("{id:long}/anular")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Anular(long id)
    {
        var result = await _mediator.Send(new AnularFacturaCommand(id));
        return Ok(ApiResponse<bool>.Ok(result, "Factura anulada."));
    }

    /// <summary>Envía la factura al SRI para autorización</summary>
    [HttpPost("{id:long}/emitir")]
    [ProducesResponseType(typeof(ApiResponse<EmitirResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Emitir(long id)
    {
        var result = await _mediator.Send(new EmitirFacturaCommand(id));
        return Ok(ApiResponse<EmitirResult>.Ok(result, result.Mensaje));
    }

    /// <summary>Descarga el RIDE (PDF) de la factura</summary>
    [HttpGet("{id:long}/ride")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> DescargarRide(long id)
    {
        var pdf = await _mediator.Send(new GenerarRideQuery(id));
        return File(pdf, "application/pdf", $"RIDE-{id}.pdf");
    }

    /// <summary>Descarga ZIP con XMLs firmados de facturas AUTORIZADAS en el período</summary>
    [HttpGet("exportar-xml")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportarXml([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        var zip = await _mediator.Send(new ExportarXmlZipQuery(desde, hasta));
        return File(zip, "application/zip", $"XMLs_{desde:yyyyMMdd}_{hasta:yyyyMMdd}.zip");
    }
}
