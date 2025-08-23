using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ThreadPilot.Vehicles.IntegrationTests;

public sealed class TestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["Security:Enabled"] = "false", // Disable security for integration tests
                ["StubVehicle:TimeoutDelayMs"] = "100", // Reduce timeout for faster tests
                ["StubVehicle:SlowDelayMs"] = "50", // Reduce slow delay for faster tests
                ["StubVehicle:Scenarios:ABC123:Type"] = "Success",
                ["StubVehicle:Scenarios:XYZ789:Type"] = "NotFound",
                ["StubVehicle:Scenarios:ERR123:Type"] = "Error",
                ["StubVehicle:Scenarios:TMO123:Type"] = "Timeout",
                ["StubVehicle:Scenarios:SLO123:Type"] = "Slow"
            }!;
            cfg.AddInMemoryCollection(dict);
        });
    }
}

