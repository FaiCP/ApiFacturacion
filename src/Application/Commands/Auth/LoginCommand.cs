using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Auth;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IUsuarioRepository usuarioRepository, IJwtService jwtService, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

        if (usuario == null || !_passwordHasher.VerifyPassword(request.Password, usuario.Password))
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
