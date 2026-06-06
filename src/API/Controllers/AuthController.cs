using API.Models;
using Application.Commands.Auth;
using Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

/// <summary>Autenticación de usuarios</summary>
[ApiController]
[Route("api/v1")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Autentica al usuario y genera un token JWT</summary>
    /// <param name="request">Credenciales de acceso (email y contraseña)</param>
    /// <returns>Token JWT y datos del usuario autenticado</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("LoginPolicy")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password));
        return Ok(ApiResponse<LoginResponseDto>.Ok(result));
    }
}
