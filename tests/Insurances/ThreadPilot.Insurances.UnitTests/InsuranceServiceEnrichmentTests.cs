using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using ThreadPilot.Insurances.Application.Clients;
using ThreadPilot.Insurances.Application.Models;
using ThreadPilot.Insurances.Application.Services;
using ThreadPilot.Insurances.Application.Contracts;
using ThreadPilot.Insurances.Domain;

namespace ThreadPilot.Insurances.UnitTests;

public class InsuranceServiceEnrichmentTests
{
    [Fact]
    public async Task Enriches_SingleCarInsurance_WithVehicleData()
    {
        // Arrange
        var insurances = new List<Insurance>
        {
            new(new Product("Car", 100, "terms"), "ABC123")
        };
        var mockInsuranceProvider = new Mock<IInsuranceProvider>();
        mockInsuranceProvider.Setup(p => p.GetInsurancesByPersonalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(insurances);

        var vehicle = new VehicleInfo("ABC123", "Volvo", "V70", 2015, "Diesel");
        var mockVehiclesApiClient = new Mock<IVehiclesApiClient>();
        mockVehiclesApiClient.Setup(c => c.GetVehicleAsync("ABC123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var sut = new InsuranceService(mockInsuranceProvider.Object, mockVehiclesApiClient.Object, NullLogger<InsuranceService>.Instance);

        // Act
        var result = await sut.GetInsurancesAsync("19640823-3234");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(InsuranceServiceError.None);
        result.Insurances.ShouldNotBeNull();
        result.Insurances.Count.ShouldBe(1);
        result.Insurances[0].VehicleRegNo.ShouldBe("ABC123");
        result.Insurances[0].Vehicle.ShouldNotBeNull();
        result.Insurances[0].Vehicle!.Make.ShouldBe("Volvo");
    }

    [Fact]
    public async Task MultipleInsurances_Parallel_Enrichment_MixedCars_And_NonCars()
    {
        // Arrange
        var insurances = new List<Insurance>
        {
            new(new Product("Home", 1200, "t"), null),
            new(new Product("Car", 800, "t"), "ABC123"),
            new(new Product("Pet", 300, "t"), null),
            new(new Product("Car", 900, "t"), "XYZ789"),
        };
        var mockInsuranceProvider = new Mock<IInsuranceProvider>();
        mockInsuranceProvider.Setup(p => p.GetInsurancesByPersonalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(insurances);

        var vehicle = new VehicleInfo("ABC123", "Volvo", "XC60", 2020, "Hybrid");
        var mockVehiclesApiClient = new Mock<IVehiclesApiClient>();
        mockVehiclesApiClient.Setup(c => c.GetVehicleAsync("ABC123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        mockVehiclesApiClient.Setup(c => c.GetVehicleAsync("XYZ789", It.IsAny<CancellationToken>()))
            .ReturnsAsync((VehicleInfo?)null); // Simulate failure

        var sut = new InsuranceService(mockInsuranceProvider.Object, mockVehiclesApiClient.Object, NullLogger<InsuranceService>.Instance);

        // Act
        var result = await sut.GetInsurancesAsync("19640823-3234");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(InsuranceServiceError.None);
        result.Insurances.ShouldNotBeNull();
        result.Insurances.Count.ShouldBe(4);

        // Home remains without vehicle
        result.Insurances[0].Vehicle.ShouldBeNull();

        // Car ABC123 enriched
        var car1 = result.Insurances[1];
        car1.VehicleRegNo.ShouldBe("ABC123");
        car1.Vehicle.ShouldNotBeNull();
        car1.Vehicle!.Model.ShouldBe("XC60");

        // Pet remains without vehicle
        result.Insurances[2].Vehicle.ShouldBeNull();

        // Car XYZ789 failed enrichment
        var car2 = result.Insurances[3];
        car2.VehicleRegNo.ShouldBe("XYZ789");
        car2.Vehicle.ShouldBeNull();
    }

    [Fact]
    public async Task DownstreamFailures_Are_Treated_As_Partial_Not_Fatal()
    {
        // Arrange
        var insurances = new List<Insurance>
        {
            new(new Product("Car", 100, "t"), "ABC123"),
            new(new Product("Car", 200, "t"), "XYZ789"),
        };
        var mockInsuranceProvider = new Mock<IInsuranceProvider>();
        mockInsuranceProvider.Setup(p => p.GetInsurancesByPersonalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(insurances);

        var mockVehiclesApiClient = new Mock<IVehiclesApiClient>();
        mockVehiclesApiClient.Setup(c => c.GetVehicleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((VehicleInfo?)null); // always fail

        var sut = new InsuranceService(mockInsuranceProvider.Object, mockVehiclesApiClient.Object, NullLogger<InsuranceService>.Instance);

        // Act
        var result = await sut.GetInsurancesAsync("19640823-3234");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(InsuranceServiceError.None);
        result.Insurances.ShouldNotBeNull();
        result.Insurances.Count.ShouldBe(2);
        result.Insurances[0].Vehicle.ShouldBeNull();
        result.Insurances[1].Vehicle.ShouldBeNull();
    }

    [Fact]
    public async Task ProviderTimeout_Returns_TimeoutError()
    {
        // Arrange
        var mockInsuranceProvider = new Mock<IInsuranceProvider>();
        mockInsuranceProvider.Setup(p => p.GetInsurancesByPersonalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException());

        var mockVehiclesApiClient = new Mock<IVehiclesApiClient>();
        var sut = new InsuranceService(mockInsuranceProvider.Object, mockVehiclesApiClient.Object, NullLogger<InsuranceService>.Instance);

        // Act
        var result = await sut.GetInsurancesAsync("any-id");

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(InsuranceServiceError.Timeout);
        result.ErrorMessage.ShouldBe("Timeout while retrieving insurances");
        result.Insurances.ShouldBeNull();
    }

    [Fact]
    public async Task ProviderGenericException_Returns_InternalError()
    {
        // Arrange
        var mockInsuranceProvider = new Mock<IInsuranceProvider>();
        mockInsuranceProvider.Setup(p => p.GetInsurancesByPersonalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB is down"));

        var mockVehiclesApiClient = new Mock<IVehiclesApiClient>();
        var sut = new InsuranceService(mockInsuranceProvider.Object, mockVehiclesApiClient.Object, NullLogger<InsuranceService>.Instance);

        // Act
        var result = await sut.GetInsurancesAsync("any-id");

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(InsuranceServiceError.InternalError);
        result.ErrorMessage.ShouldBe("Error while retrieving insurances");
        result.Insurances.ShouldBeNull();
    }

    [Fact]
    public async Task Enrichment_Throws_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        var insurances = new List<Insurance> { new(new Product("Car", 100, "t"), "ABC123") };
        var mockInsuranceProvider = new Mock<IInsuranceProvider>();
        mockInsuranceProvider.Setup(p => p.GetInsurancesByPersonalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(insurances);

        var mockVehiclesApiClient = new Mock<IVehiclesApiClient>();
        var cts = new CancellationTokenSource();
        mockVehiclesApiClient.Setup(c => c.GetVehicleAsync(It.IsAny<string>(), cts.Token))
            .ThrowsAsync(new OperationCanceledException());

        var sut = new InsuranceService(mockInsuranceProvider.Object, mockVehiclesApiClient.Object, NullLogger<InsuranceService>.Instance);

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() => sut.GetInsurancesAsync("any-id", cts.Token));
    }

    [Fact]
    public async Task Enrichment_Swallows_Generic_Exception()
    {
        // Arrange
        var insurances = new List<Insurance> { new(new Product("Car", 100, "t"), "ABC123") };
        var mockInsuranceProvider = new Mock<IInsuranceProvider>();
        mockInsuranceProvider.Setup(p => p.GetInsurancesByPersonalIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(insurances);

        var mockVehiclesApiClient = new Mock<IVehiclesApiClient>();
        mockVehiclesApiClient.Setup(c => c.GetVehicleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("API is down"));

        var sut = new InsuranceService(mockInsuranceProvider.Object, mockVehiclesApiClient.Object, NullLogger<InsuranceService>.Instance);

        // Act
        var result = await sut.GetInsurancesAsync("any-id");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Insurances.ShouldNotBeNull();
        result.Insurances.Count.ShouldBe(1);
        result.Insurances[0].Vehicle.ShouldBeNull(); // Enrichment failed, but no exception was thrown
    }
}

