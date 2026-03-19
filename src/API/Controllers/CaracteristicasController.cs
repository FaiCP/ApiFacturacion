using API.Models;
using Application.Commands.Hardware;
using Application.DTOs.Hardware;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Características técnicas de computadoras</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CaracteristicasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CaracteristicasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Crea las características técnicas de una computadora</summary>
    /// <param name="request">Datos de características (RAM, ROM, Procesador)</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Crear([FromBody] CaracteristicaComputadoraDto request)
    {
        var result = await _mediator.Send(new CreateCaracteristicaCommand(
            request.IdEquipo,
            request.Ram,
            request.Rom,
            request.Procesador));
        return Ok(ApiResponse<bool>.Ok(result));
    }
}
