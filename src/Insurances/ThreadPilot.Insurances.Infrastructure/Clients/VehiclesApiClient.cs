using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ThreadPilot.Insurances.Application.Clients;
using ThreadPilot.Insurances.Application.Models;

namespace ThreadPilot.Insurances.Infrastructure.Clients;

public sealed class VehiclesApiClientOptions
{
    public const string SectionName = "VehiclesApi";
    public string BaseAddress { get; set; } = "http://localhost"; // override in tests/config
}

public sealed class VehiclesApiClient : IVehiclesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VehiclesApiClient> _logger;

    public VehiclesApiClient(HttpClient httpClient, IOptions<VehiclesApiClientOptions> options, ILogger<VehiclesApiClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        _httpClient = httpClient;
        _httpClient.BaseAddress = new System.Uri(options.Value.BaseAddress, System.UriKind.Absolute);
        _logger = logger;
    }

    public async Task<VehicleInfo?> GetVehicleAsync(string registrationNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(new Uri($"/api/vehicles/{registrationNumber}", UriKind.Relative), cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var dto = await response.Content.ReadFromJsonAsync<VehicleInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return dto;
        }
        catch (HttpRequestException)
        {
            // Swallow to allow partial enrichment; observability via upstream
            return null;
        }
    }
}

