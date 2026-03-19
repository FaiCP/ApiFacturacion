using Application.Commands.Hardware;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace GestorAdmi.Tests.Unit.Handlers;

public class CreateHardwareCommandHandlerTests
{
    private readonly Mock<IHardwareRepository> _hardwareRepoMock = new();
    private readonly Mock<IGenericRepository<CaracteristicaComputadora>> _caracteristicaRepoMock = new();

    private CreateHardwareCommandHandler CreateHandler() =>
        new(_hardwareRepoMock.Object, _caracteristicaRepoMock.Object);

    [Fact]
    public async Task Handle_CreatesHardwareAndReturnsId()
    {
        // Arrange
        var command = new CreateHardwareCommand(
            Ubicacion: "Oficina 1",
            Descripcion: "Laptop corporativa",
            NombreDispositivo: "Laptop",
            Marca: "Dell",
            Modelo: "XPS 15",
            CodigoCne: "CNE-001",
            IdEquipo: "EQ-001",
            Estado: "Activo",
            Ram: "",
            Rom: "",
            Procesador: "",
            Valor: 1200.00);

        var savedHardware = new Hardware { Id = 42 };
        _hardwareRepoMock.Setup(r => r.AddAsync(It.IsAny<Hardware>()))
            .ReturnsAsync(savedHardware);

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(42);
        _hardwareRepoMock.Verify(r => r.AddAsync(It.Is<Hardware>(h =>
            h.Marca == "Dell" &&
            h.Modelo == "XPS 15" &&
            h.Borrado == false)), Times.Once);
    }

    [Fact]
    public async Task Handle_WithRam_CreatesCaracteristica()
    {
        // Arrange
        var command = new CreateHardwareCommand(
            Ubicacion: "Sala Técnica",
            Descripcion: "Servidor",
            NombreDispositivo: "Server",
            Marca: "HP",
            Modelo: "ProLiant",
            CodigoCne: "CNE-002",
            IdEquipo: "EQ-002",
            Estado: "Activo",
            Ram: "32GB",
            Rom: "1TB SSD",
            Procesador: "Intel Xeon",
            Valor: null);

        var savedHardware = new Hardware { Id = 10 };
        _hardwareRepoMock.Setup(r => r.AddAsync(It.IsAny<Hardware>())).ReturnsAsync(savedHardware);
        _caracteristicaRepoMock.Setup(r => r.AddAsync(It.IsAny<CaracteristicaComputadora>()))
            .ReturnsAsync(new CaracteristicaComputadora());

        // Act
        var result = await CreateHandler().Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(10);
        _caracteristicaRepoMock.Verify(r => r.AddAsync(It.Is<CaracteristicaComputadora>(c =>
            c.Ram == "32GB" &&
            c.Rom == "1TB SSD" &&
            c.Procesador == "Intel Xeon" &&
            c.HardwareId == 10)), Times.Once);
    }

    [Fact]
    public async Task Handle_WithoutRamRomProcesador_DoesNotCreateCaracteristica()
    {
        // Arrange
        var command = new CreateHardwareCommand(
            Ubicacion: "Bodega",
            Descripcion: "Monitor",
            NombreDispositivo: "Monitor",
            Marca: "LG",
            Modelo: "27UK850",
            CodigoCne: "CNE-003",
            IdEquipo: "EQ-003",
            Estado: "Activo",
            Ram: "",
            Rom: "",
            Procesador: "",
            Valor: 350.00);

        _hardwareRepoMock.Setup(r => r.AddAsync(It.IsAny<Hardware>()))
            .ReturnsAsync(new Hardware { Id = 7 });

        // Act
        await CreateHandler().Handle(command, CancellationToken.None);

        // Assert — CaracteristicaComputadora never created for peripherals
        _caracteristicaRepoMock.Verify(r => r.AddAsync(It.IsAny<CaracteristicaComputadora>()), Times.Never);
    }
}
