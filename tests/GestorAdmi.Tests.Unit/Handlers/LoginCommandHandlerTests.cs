using Application.Commands.Auth;
using Application.DTOs.Auth;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace GestorAdmi.Tests.Unit.Handlers;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock = new();
    private readonly Mock<IJwtService> _jwtServiceMock = new();

    private LoginCommandHandler CreateHandler() =>
        new(_usuarioRepoMock.Object, _jwtServiceMock.Object);

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var password = "secret123";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Admin",
            Email = "admin@test.com",
            Password = hash,
            Cargo = "Administrador"
        };

        _usuarioRepoMock.Setup(r => r.GetByEmailAsync("admin@test.com"))
            .ReturnsAsync(usuario);
        _jwtServiceMock.Setup(j => j.GenerateToken(usuario))
            .Returns("fake-jwt-token");

        var command = new LoginCommand("admin@test.com", password);

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
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

        // Act
        var act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
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
            Password = BCrypt.Net.BCrypt.HashPassword("correct_password")
        };

        _usuarioRepoMock.Setup(r => r.GetByEmailAsync("admin@test.com"))
            .ReturnsAsync(usuario);

        var command = new LoginCommand("admin@test.com", "wrong_password");

        var act = async () => await CreateHandler().Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");
    }
}
