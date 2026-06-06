using API.Models;
using Application.Commands.NotasCredito;
using Application.DTOs.NotasCredito;
using Application.Queries.NotasCredito;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Notas de Crédito electrónicas</summary>
[ApiController]
[Route("api/v1/notas-credito")]
[Authorize]
public class NotasCreditoController : ControllerBase
{
    private readonly IMediator _mediator;
    public NotasCreditoController(IMediator mediator) => _mediator = mediator;

    [HttpGet("LeerTodo")]
    public async Task<IActionResult> LeerTodo([FromQuery] int cantidad = 10, [FromQuery] int pagina = 1,
        [FromQuery] EstadoSRI? estado = null, [FromQuery] long? facturaId = null)
    {
        var result = await _mediator.Send(new GetNotasCreditoQuery(cantidad, pagina, estado, facturaId));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<NotaCreditoDto>>.Ok(result));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetNotaCreditoByIdQuery(id));
        return Ok(ApiResponse<NotaCreditoDto>.Ok(result));
    }

    /// <summary>Crea Nota de Crédito en estado BORRADOR (requiere factura AUTORIZADA)</summary>
    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] NotaCreditoCreateDto dto)
    {
        var id = await _mediator.Send(new CreateNotaCreditoCommand(dto.FacturaId, dto.Motivo, dto.FechaEmision, dto.Detalles));
        return StatusCode(201, ApiResponse<long>.Ok(id, "Nota de Crédito creada en estado BORRADOR."));
    }

    /// <summary>Envía Nota de Crédito al SRI para autorización</summary>
    [HttpPost("{id:long}/emitir")]
    public async Task<IActionResult> Emitir(long id)
    {
        var result = await _mediator.Send(new EmitirNotaCreditoCommand(id));
        return Ok(ApiResponse<EmitirNotaCreditoResult>.Ok(result, result.Mensaje));
    }
}
