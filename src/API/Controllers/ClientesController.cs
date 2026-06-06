using API.Models;
using Application.Commands.Clientes;
using Application.DTOs.Clientes;
using Application.Queries.Clientes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Gestión de clientes</summary>
[ApiController]
[Route("api/v1/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listado paginado de clientes</summary>
    [HttpGet("LeerTodo")]
    [ProducesResponseType(typeof(ApiResponse<Application.Common.PaginatedResponse<ClienteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LeerTodo(
        [FromQuery] int cantidad = 10,
        [FromQuery] int pagina = 1,
        [FromQuery] string busqueda = "")
    {
        var result = await _mediator.Send(new GetClientesQuery(cantidad, pagina, busqueda));
        return Ok(ApiResponse<Application.Common.PaginatedResponse<ClienteDto>>.Ok(result));
    }

    /// <summary>Obtiene un cliente por Id</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetClienteByIdQuery(id));
        return Ok(ApiResponse<ClienteDto>.Ok(result));
    }

    /// <summary>Crea un cliente</summary>
    [HttpPost("Crear")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear([FromBody] ClienteCreateDto dto)
    {
        var id = await _mediator.Send(new CreateClienteCommand(
            dto.TipoIdentificacion, dto.NumeroIdentificacion,
            dto.RazonSocial, dto.Email, dto.Telefono, dto.Direccion));

        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<long>.Ok(id, "Cliente creado correctamente."));
    }

    /// <summary>Actualiza un cliente</summary>
    [HttpPut("Actualizar/{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ClienteCreateDto dto)
    {
        var result = await _mediator.Send(new UpdateClienteCommand(
            id, dto.RazonSocial, dto.Email, dto.Telefono, dto.Direccion));

        return Ok(ApiResponse<bool>.Ok(result, "Cliente actualizado correctamente."));
    }
}
