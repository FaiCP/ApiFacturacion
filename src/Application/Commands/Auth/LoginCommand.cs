using Application.DTOs.Auth;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Auth;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IUsuarioRepository usuarioRepository, IJwtService jwtService)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        var token = _jwtService.GenerateToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email ?? string.Empty,
            Cargo = usuario.Cargo ?? string.Empty
        };
    }
}
