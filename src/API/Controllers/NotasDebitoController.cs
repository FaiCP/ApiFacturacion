using API.Models;
using Application.Commands.NotasDebito;
using Application.DTOs.NotasDebito;
using Application.Queries.NotasDebito;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Notas de Débito electrónicas</summary>
[ApiController]
[Route("api/v1/notas-debito")]
[Authorize]
public class NotasDebitoController : ControllerBase
{
    private readonly IMediator _mediator;
    public NotasDebitoController(IMediator mediator) => _mediator = mediator;

    [HttpGet("LeerTodo")]
    public async Task<IActionResult> LeerTodo([FromQuery] int cantidad = 10, [FromQuery] int pagina = 1,
        [FromQuery] EstadoSRI? estado = null, [FromQuery] long? facturaId = null)
    {
        var result = await _mediator.Send(new GetNotasDebitoQuery(cantidad, pagina, estado, facturaId));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<NotaDebitoDto>>.Ok(result));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetNotaDebitoByIdQuery(id));
        return Ok(ApiResponse<NotaDebitoDto>.Ok(result));
    }

    /// <summary>Crea Nota de Débito en estado BORRADOR (requiere factura AUTORIZADA)</summary>
    [HttpPost("Crear")]
    public async Task<IActionResult> Crear([FromBody] NotaDebitoCreateDto dto)
    {
        var id = await _mediator.Send(new CreateNotaDebitoCommand(dto.FacturaId, dto.FechaEmision, dto.Motivos));
        return StatusCode(201, ApiResponse<long>.Ok(id, "Nota de Débito creada en estado BORRADOR."));
    }

    /// <summary>Envía Nota de Débito al SRI para autorización</summary>
    [HttpPost("{id:long}/emitir")]
    public async Task<IActionResult> Emitir(long id)
    {
        var result = await _mediator.Send(new EmitirNotaDebitoCommand(id));
        return Ok(ApiResponse<EmitirNotaDebitoResult>.Ok(result, result.Mensaje));
    }
}
