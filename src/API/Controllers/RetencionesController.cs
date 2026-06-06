using API.Models;
using Application.Commands.Retenciones;
using Application.DTOs.Retenciones;
using Application.Queries.Retenciones;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Comprobantes de Retención electrónicos</summary>
[ApiController]
[Route("api/v1/retenciones")]
[Authorize]
public class RetencionesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RetencionesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("LeerTodo")]
    public async Task<IActionResult> LeerTodo([FromQuery] int cantidad = 10, [FromQuery] int pagina = 1,
        [FromQuery] EstadoSRI? estado = null, [FromQuery] long? facturaId = null)
    {
        var result = await _mediator.Send(new GetRetencionesQuery(cantidad, pagina, estado, facturaId));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<RetencionDto>>.Ok(result));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetRetencionByIdQuery(id));
        return Ok(ApiResponse<RetencionDto>.Ok(result));
    }

    /// <summary>Crea Comprobante de Retención en estado BORRADOR</summary>
    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] RetencionCreateDto dto)
    {
        var id = await _mediator.Send(new CreateRetencionCommand(
            dto.FacturaId, dto.SujetoRetenidoId, dto.FechaEmision, dto.PeriodoFiscal, dto.Detalles));
        return StatusCode(201, ApiResponse<long>.Ok(id, "Retención creada en estado BORRADOR."));
    }

    /// <summary>Envía Retención al SRI para autorización</summary>
    [HttpPost("{id:long}/emitir")]
    public async Task<IActionResult> Emitir(long id)
    {
        var result = await _mediator.Send(new EmitirRetencionCommand(id));
        return Ok(ApiResponse<EmitirRetencionResult>.Ok(result, result.Mensaje));
    }
}
