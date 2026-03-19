using Application.DTOs.Hardware;
using Application.Queries.Hardware;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace GestorAdmi.Tests.Unit.Handlers;

public class GetHardwareQueryHandlerTests
{
    private readonly Mock<IHardwareRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private GetHardwareQueryHandler CreateHandler() =>
        new(_repoMock.Object, _mapperMock.Object);

    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        // Arrange
        var hardwareList = new List<Hardware>
        {
            new() { Id = 1, NombreDispositivo = "Laptop", Marca = "Dell", Modelo = "XPS", Estado = "Activo" },
            new() { Id = 2, NombreDispositivo = "Monitor", Marca = "LG", Modelo = "27UK", Estado = "Activo" }
        };
        var dtoList = hardwareList.Select(h => new HardwareDto
        {
            Id = h.Id, NombreDispositivo = h.NombreDispositivo, Marca = h.Marca
        }).ToList();

        _repoMock.Setup(r => r.SearchAsync("", 1, 10)).ReturnsAsync(hardwareList);
        _repoMock.Setup(r => r.CountSearchAsync("")).ReturnsAsync(2);
        _mapperMock.Setup(m => m.Map<List<HardwareDto>>(hardwareList)).Returns(dtoList);

        var query = new GetHardwareQuery(10, 1, "");

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_PageZeroOrNegative_NormalizesToOne()
    {
        // Arrange
        _repoMock.Setup(r => r.SearchAsync("", 1, 10)).ReturnsAsync(new List<Hardware>());
        _repoMock.Setup(r => r.CountSearchAsync("")).ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<List<HardwareDto>>(It.IsAny<List<Hardware>>()))
            .Returns(new List<HardwareDto>());

        var query = new GetHardwareQuery(10, 0, "");

        // Act
        var result = await CreateHandler().Handle(query, CancellationToken.None);

        // Assert — repository is called with page=1 when query.Pagina=0
        _repoMock.Verify(r => r.SearchAsync("", 1, 10), Times.Once);
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithBusqueda_PassesSearchTermToRepository()
    {
        // Arrange
        _repoMock.Setup(r => r.SearchAsync("Dell", 1, 5)).ReturnsAsync(new List<Hardware>());
        _repoMock.Setup(r => r.CountSearchAsync("Dell")).ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<List<HardwareDto>>(It.IsAny<List<Hardware>>()))
            .Returns(new List<HardwareDto>());

        var query = new GetHardwareQuery(5, 1, "Dell");

        // Act
        await CreateHandler().Handle(query, CancellationToken.None);

        // Assert
        _repoMock.Verify(r => r.SearchAsync("Dell", 1, 5), Times.Once);
        _repoMock.Verify(r => r.CountSearchAsync("Dell"), Times.Once);
    }
}
