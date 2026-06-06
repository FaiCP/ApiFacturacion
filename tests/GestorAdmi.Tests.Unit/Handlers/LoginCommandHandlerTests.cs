using Application.Commands.Auth;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace GestorAdmi.Tests.Unit.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

    private LoginCommandHandler CreateHandler() =>
        new(_usuarioRepoMock.Object, _jwtServiceMock.Object, _passwordHasherMock.Object);

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResponse()
    {
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Admin",
            Email = "admin@test.com",
            Password = "hashed_password",
            Cargo = "Administrador"
        };

        _usuarioRepoMock.Setup(r => r.GetByEmailAsync("admin@test.com"))
            .ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.VerifyPassword("secret123", "hashed_password"))
            .Returns(true);
        _jwtServiceMock.Setup(j => j.GenerateToken(usuario))
            .Returns("fake-jwt-token");

        var command = new LoginCommand("admin@test.com", "secret123");

        var result = await CreateHandler().Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Token.Should().Be("fake-jwt-token");
        result.Id.Should().Be(1);
        result.Email.Should().Be("admin@test.com");
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorized()
    {
        _usuarioRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Usuario?)null);

        var command = new LoginCommand("noexiste@test.com", "any");

        var act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        var usuario = new Usuario
        {
            Id = 1,
            Email = "admin@test.com",
            Password = "hashed_password"
        };

        _usuarioRepoMock.Setup(r => r.GetByEmailAsync("admin@test.com"))
            .ReturnsAsync(usuario);
        _passwordHasherMock.Setup(p => p.VerifyPassword("wrong_password", "hashed_password"))
            .Returns(false);

        var command = new LoginCommand("admin@test.com", "wrong_password");

        var act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }
}