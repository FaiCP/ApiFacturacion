using API.Models;
using Application.Commands.ConfiguracionSRI;
using Application.DTOs.ConfiguracionSRI;
using Application.Queries.ConfiguracionSRI;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>Configuración del certificado digital y ambiente SRI</summary>
[ApiController]
[Route("api/v1/configuracion-sri")]
[Authorize(Policy = "RequireAdminRole")]
public class ConfiguracionSRIController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConfiguracionSRIController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene la configuración SRI activa de un emisor</summary>
    [HttpGet("{emisorId:long}")]
    [ProducesResponseType(typeof(ApiResponse<ConfiguracionSRIDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(long emisorId)
    {
        var result = await _mediator.Send(new GetConfiguracionSRIQuery(emisorId));
        return Ok(ApiResponse<ConfiguracionSRIDto?>.Ok(result));
    }

    /// <summary>Guarda o actualiza el certificado digital y ambiente SRI de un emisor</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Guardar([FromBody] ConfiguracionSRICreateDto dto)
    {
        var id = await _mediator.Send(new SaveConfiguracionSRICommand(
            dto.EmisorId, dto.CertificadoBase64, dto.PasswordCertificado, dto.Ambiente));

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<long>.Ok(id, "Configuración SRI guardada. Certificado validado correctamente."));
    }

    /// <summary>Sube certificado .p12 como archivo para un emisor</summary>
    [HttpPost("upload-certificado")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadCertificado(
        IFormFile archivo,
        [FromForm] long emisorId,
        [FromForm] string password,
        [FromForm] Domain.Enums.AmbienteSRI ambiente = Domain.Enums.AmbienteSRI.Pruebas)
    {
        if (archivo.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Archivo vacío."));

        using var ms = new MemoryStream();
        await archivo.CopyToAsync(ms);
        var base64 = Convert.ToBase64String(ms.ToArray());

        var id = await _mediator.Send(new SaveConfiguracionSRICommand(emisorId, base64, password, ambiente));
        return Ok(ApiResponse<long>.Ok(id, "Certificado cargado y validado correctamente."));
    }
}
