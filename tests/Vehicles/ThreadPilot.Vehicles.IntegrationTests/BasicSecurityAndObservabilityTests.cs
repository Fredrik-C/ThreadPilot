using System.Net;
using Shouldly;

namespace ThreadPilot.Vehicles.IntegrationTests;

public sealed class BasicSecurityAndObservabilityTests(TestWebAppFactory factory) : IClassFixture<TestWebAppFactory>
{
    private readonly TestWebAppFactory factory = factory;

    [Fact]
    public async Task HealthEndpointsReturnOk()
    {
        var client = factory.CreateClient();
        var live = await client.GetAsync(new Uri("/health/live", UriKind.Relative));
        var ready = await client.GetAsync(new Uri("/health/ready", UriKind.Relative));
        live.StatusCode.ShouldBe(HttpStatusCode.OK);
        ready.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SwaggerDevelopmentReturnsOk()
    {
        var client = factory.CreateClient();
        var resp = await client.GetAsync(new Uri("/swagger/v1/swagger.json", UriKind.Relative));
        resp.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DevAuthDisabledAllowsRoot()
    {
        var client = factory.CreateClient();
        var resp = await client.GetAsync(new Uri("/", UriKind.Relative));
        resp.StatusCode.ShouldBe(HttpStatusCode.OK);
        resp.Headers.Contains("X-Correlation-ID").ShouldBeTrue();
    }
}