using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace ThreadPilot.Insurances.IntegrationTests;

public class InsurancesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public InsurancesApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetInsurances_WithValidPersonalId_ShouldReturnInsurances()
    {
        // Arrange
        var personalId = "19640823-3234";

        // Act
        var response = await _client.GetAsync(new Uri($"/api/insurances/{personalId}", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var insurances = await response.Content.ReadFromJsonAsync<object[]>();
        insurances.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetInsurances_WithInvalidPersonalId_ShouldReturnBadRequest()
    {
        // Arrange
        var personalId = "invalid";

        // Act
        var response = await _client.GetAsync(new Uri($"/api/insurances/{personalId}", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetInsurances_WithEmptyPersonalId_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync(new Uri("/api/insurances", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}