using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ThreadPilot.Vehicles.Domain;

namespace ThreadPilot.Vehicles.Infrastructure.Providers;

public class StubVehicleInfoProvider : IVehicleInfoProvider
{
    private readonly StubVehicleOptions _options;

    public StubVehicleInfoProvider(IOptions<StubVehicleOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;
    }

    public async Task<Vehicle?> GetVehicleInfoAsync(string registrationNumber, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(registrationNumber);

        // Check for magic inputs first
        if (registrationNumber.StartsWith("TIMEOUT-", StringComparison.OrdinalIgnoreCase))
        {
            await Task.Delay(TimeSpan.FromMilliseconds(_options.TimeoutDelayMs), cancellationToken).ConfigureAwait(false);
            throw new TimeoutException("Simulated timeout from external vehicle system");
        }

        if (registrationNumber.StartsWith("ERROR-", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Simulated error from external vehicle system");
        }

        if (registrationNumber.StartsWith("SLOW-", StringComparison.OrdinalIgnoreCase))
        {
            await Task.Delay(TimeSpan.FromMilliseconds(_options.SlowDelayMs), cancellationToken).ConfigureAwait(false);
            return new Vehicle(registrationNumber, "Volvo", "V60", 2019, "Petrol");
        }

        if (registrationNumber.StartsWith("NOTFOUND-", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // Check for DI-seeded scenarios
        if (_options.Scenarios.TryGetValue(registrationNumber, out var scenario))
        {
            switch (scenario.Type)
            {
                case ScenarioType.Timeout:
                    await Task.Delay(TimeSpan.FromMilliseconds(scenario.DelayMs ?? _options.TimeoutDelayMs), cancellationToken).ConfigureAwait(false);
                    throw new TimeoutException(scenario.ErrorMessage ?? "Simulated timeout from external vehicle system");
                case ScenarioType.Error:
                    throw new InvalidOperationException(scenario.ErrorMessage ?? "Simulated error from external vehicle system");
                case ScenarioType.Slow:
                    await Task.Delay(TimeSpan.FromMilliseconds(scenario.DelayMs ?? _options.SlowDelayMs), cancellationToken).ConfigureAwait(false);
                    return scenario.Payload ?? CreateDefaultVehicle(registrationNumber);
                case ScenarioType.NotFound:
                    return null;
                case ScenarioType.Success:
                    return scenario.Payload ?? CreateDefaultVehicle(registrationNumber);
            }
        }

        // Default success scenario
        return CreateDefaultVehicle(registrationNumber);
    }

    private static Vehicle CreateDefaultVehicle(string registrationNumber)
    {
        return new Vehicle(registrationNumber, "Volvo", "V60", 2019, "Petrol");
    }
}

public class StubVehicleOptions
{
    public const string SectionName = "StubVehicle";

    public Dictionary<string, Scenario> Scenarios { get; } = new();
    public int TimeoutDelayMs { get; set; } = 2000;
    public int SlowDelayMs { get; set; } = 1000;
}

public record Scenario(
    ScenarioType Type,
    int? DelayMs = null,
    Vehicle? Payload = null,
    string? ErrorMessage = null);

public enum ScenarioType
{
    Success,
    NotFound,
    Timeout,
    Error,
    Slow
}