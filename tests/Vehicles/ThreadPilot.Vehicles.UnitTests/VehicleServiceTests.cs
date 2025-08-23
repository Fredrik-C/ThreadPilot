using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using ThreadPilot.Vehicles.Application.Services;
using ThreadPilot.Vehicles.Application.Contracts;
using ThreadPilot.Vehicles.Domain;
using Xunit;

namespace ThreadPilot.Vehicles.UnitTests;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleInfoProvider> _mockProvider;
    private readonly VehicleService _vehicleService;

    public VehicleServiceTests()
    {
        _mockProvider = new Mock<IVehicleInfoProvider>();
        _vehicleService = new VehicleService(_mockProvider.Object);
    }

    [Fact]
    public async Task GetVehicleAsyncShouldReturnSuccessResultWhenVehicleIsFound()
    {
        // Arrange
        var registrationNumber = "ABC123";
        var expectedVehicle = new Vehicle(registrationNumber, "Volvo", "V60", 2019, "Petrol");
        _mockProvider.Setup(p => p.GetVehicleInfoAsync(registrationNumber, CancellationToken.None))
            .ReturnsAsync(expectedVehicle);

        // Act
        var result = await _vehicleService.GetVehicleAsync(registrationNumber);

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
        _mockProvider.Setup(p => p.GetVehicleInfoAsync(registrationNumber, CancellationToken.None))
            .ReturnsAsync((Vehicle?)null);

        // Act
        var result = await _vehicleService.GetVehicleAsync(registrationNumber);

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
        var result = await _vehicleService.GetVehicleAsync(registrationNumber);

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
        _mockProvider.Setup(p => p.GetVehicleInfoAsync(registrationNumber, CancellationToken.None))
            .ThrowsAsync(new TimeoutException());

        // Act & Assert
        await Should.ThrowAsync<TimeoutException>(async () => await _vehicleService.GetVehicleAsync(registrationNumber));
    }

    [Fact]
    public async Task GetVehicleAsyncShouldThrowInvalidOperationExceptionWhenProviderThrowsException()
    {
        // Arrange
        var registrationNumber = "ABC123";
        _mockProvider.Setup(p => p.GetVehicleInfoAsync(registrationNumber, CancellationToken.None))
            .ThrowsAsync(new InvalidOperationException("Test error"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () => await _vehicleService.GetVehicleAsync(registrationNumber));
    }
}