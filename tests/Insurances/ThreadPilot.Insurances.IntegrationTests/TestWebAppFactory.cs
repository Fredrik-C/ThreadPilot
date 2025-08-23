using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ThreadPilot.Insurances.Api;

namespace ThreadPilot.Insurances.IntegrationTests;

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
                ["Security:Enabled"] = "true"
            }!;
            cfg.AddInMemoryCollection(dict);
        });
    }
}

