using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ThreadPilot.Vehicles.Api;

namespace ThreadPilot.Vehicles.IntegrationTests;

[SuppressMessage("Design", "CA1515", Justification = "Public fixture type used by xUnit test discovery")]
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
                ["StubVehicle:SlowDelayMs"] = "50" // Reduce slow delay for faster tests
            }!;
            cfg.AddInMemoryCollection(dict);
        });
    }
}

