using System.Net;
using System.Net.Http.Json;
using Shouldly;
using ThreadPilot.Vehicles.Api.DTOs;

namespace ThreadPilot.Vehicles.IntegrationTests;

public sealed class VehicleEndpointTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory factory = factory;

    [Fact]
    public async Task GetVehicleByRegistrationNumber_ShouldReturnVehicle_WhenRegistrationNumberIsValidAndFound()
    {
        // Arrange
        var client = factory.CreateClient();
        var registrationNumber = "ABC123";

        // Act
        var response = await client.GetAsync(new Uri($"/api/vehicles/{registrationNumber}", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var vehicle = await response.Content.ReadFromJsonAsync<VehicleDto>();
        _ = vehicle.ShouldNotBeNull();
        vehicle.RegNo.ShouldBe(registrationNumber);
        vehicle.Make.ShouldBe("Volvo");
        vehicle.Model.ShouldBe("V60");
        vehicle.Year.ShouldBe(2019);
        vehicle.FuelType.ShouldBe("Petrol");
    }

    [Fact]
    public async Task GetVehicleByRegistrationNumber_ShouldReturnNotFound_WhenVehicleIsNotFound()
    {
        // Arrange
        var client = factory.CreateClient();
        var registrationNumber = "NOTFOUND-XYZ789";

        // Act
        var response = await client.GetAsync(new Uri($"/api/vehicles/{registrationNumber}", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var problemDetails = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        _ = problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Vehicle Not Found");
    }

    [Fact]
    public async Task GetVehicleByRegistrationNumber_ShouldReturnBadRequest_WhenRegistrationNumberIsInvalid()
    {
        // Arrange
        var client = factory.CreateClient();
        var registrationNumber = ""; // Invalid registration number

        // Act
        var response = await client.GetAsync(new Uri($"/api/vehicles/{registrationNumber}", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        _ = problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Invalid Registration Number");
    }

    [Fact]
    public async Task GetVehicleByRegistrationNumber_ShouldReturnServiceUnavailable_WhenProviderTimesOut()
    {
        // Arrange
        var client = factory.CreateClient();
        var registrationNumber = "TIMEOUT-ABC123";

        // Act
        var response = await client.GetAsync(new Uri($"/api/vehicles/{registrationNumber}", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
        var problemDetails = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        _ = problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Service Unavailable");
    }

    [Fact]
    public async Task GetVehicleByRegistrationNumber_ShouldReturnInternalServerError_WhenProviderThrowsError()
    {
        // Arrange
        var client = factory.CreateClient();
        var registrationNumber = "ERROR-XYZ789";

        // Act
        var response = await client.GetAsync(new Uri($"/api/vehicles/{registrationNumber}", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        var problemDetails = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        _ = problemDetails.ShouldNotBeNull();
        problemDetails.Title.ShouldBe("Internal Server Error");
    }
}