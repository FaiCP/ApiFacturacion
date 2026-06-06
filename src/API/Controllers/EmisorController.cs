using API.Models;
using Application.Commands.Emisor;
using Application.DTOs.Emisor;
using Application.Queries.Emisor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Configuración del emisor (empresa)</summary>
[ApiController]
[Route("api/v1/emisor")]
[Authorize]
public class EmisorController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmisorController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene la configuración del emisor activo</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<EmisorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetEmisorQuery());
        return Ok(ApiResponse<EmisorDto>.Ok(result));
    }

    /// <summary>Crea el emisor (solo la primera vez)</summary>
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear([FromBody] EmisorCreateDto dto)
    {
        var id = await _mediator.Send(new CreateEmisorCommand(
            dto.Ruc, dto.RazonSocial, dto.NombreComercial, dto.Direccion,
            dto.Telefono, dto.Email, dto.ObligadoContabilidad, dto.Ambiente,
            dto.SerieEstablecimiento, dto.SeriePuntoEmision, dto.LogoBase64));

        return CreatedAtAction(nameof(Get), ApiResponse<long>.Ok(id, "Emisor creado correctamente."));
    }

    /// <summary>Actualiza el emisor</summary>
    [HttpPut("{id:long}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] EmisorCreateDto dto)
    {
        var result = await _mediator.Send(new UpdateEmisorCommand(
            id, dto.RazonSocial, dto.NombreComercial, dto.Direccion,
            dto.Telefono, dto.Email, dto.ObligadoContabilidad, dto.Ambiente,
            dto.SerieEstablecimiento, dto.SeriePuntoEmision, dto.LogoBase64));

        return Ok(ApiResponse<bool>.Ok(result, "Emisor actualizado correctamente."));
    }
}
