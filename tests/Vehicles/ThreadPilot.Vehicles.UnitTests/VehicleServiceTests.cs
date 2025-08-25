using Moq;
using Shouldly;
using ThreadPilot.Vehicles.Application.Services;
using ThreadPilot.Vehicles.Application.Contracts;
using ThreadPilot.Vehicles.Domain;

namespace ThreadPilot.Vehicles.UnitTests;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleInfoProvider> mockProvider;
    private readonly VehicleService sut;

    public VehicleServiceTests()
    {
        mockProvider = new Mock<IVehicleInfoProvider>();
        sut = new VehicleService(mockProvider.Object);
    }

    [Fact]
    public async Task GetVehicleAsyncShouldReturnSuccessResultWhenVehicleIsFound()
    {
        // Arrange
        var registrationNumber = "ABC123";
        var expectedVehicle = new Vehicle(registrationNumber, "Volvo", "V60", 2019, "Petrol");
        mockProvider.Setup(p => p.GetVehicleInfoAsync(registrationNumber, CancellationToken.None))
            .ReturnsAsync(expectedVehicle);

        // Act
        var result = await sut.GetVehicleAsync(registrationNumber);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Vehicle.ShouldBe(expectedVehicle);
        result.Error.ShouldBe(VehicleServiceError.None);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public async Task GetVehicleAsyncShouldReturnNotFoundResultWhenVehicleIsNotFound()
    {
        // Arrange
        var registrationNumber = "XYZ789";
        mockProvider.Setup(p => p.GetVehicleInfoAsync(registrationNumber, CancellationToken.None))
            .ReturnsAsync((Vehicle?)null);

        // Act
        var result = await sut.GetVehicleAsync(registrationNumber);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Vehicle.ShouldBeNull();
        result.Error.ShouldBe(VehicleServiceError.NotFound);
        result.ErrorMessage.ShouldBe("Vehicle not found");
    }

    [Fact]
    public async Task GetVehicleAsyncShouldReturnInvalidResultWhenRegistrationNumberIsInvalid()
    {
        // Arrange
        var registrationNumber = ""; // Invalid registration number

        // Act
        var result = await sut.GetVehicleAsync(registrationNumber);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Vehicle.ShouldBeNull();
        result.Error.ShouldBe(VehicleServiceError.InvalidRegistrationNumber);
        result.ErrorMessage.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetVehicleAsyncShouldThrowTimeoutExceptionWhenProviderThrowsTimeoutException()
    {
        // Arrange
        var registrationNumber = "ABC123";
        mockProvider.Setup(p => p.GetVehicleInfoAsync(registrationNumber, CancellationToken.None))
            .ThrowsAsync(new TimeoutException());

        // Act & Assert
        await Should.ThrowAsync<TimeoutException>(async () => await sut.GetVehicleAsync(registrationNumber));
    }

    [Fact]
    public async Task GetVehicleAsyncShouldThrowInvalidOperationExceptionWhenProviderThrowsException()
    {
        // Arrange
        var registrationNumber = "ABC123";
        mockProvider.Setup(p => p.GetVehicleInfoAsync(registrationNumber, CancellationToken.None))
            .ThrowsAsync(new InvalidOperationException("Test error"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () => await sut.GetVehicleAsync(registrationNumber));
    }
}
