using Microsoft.Extensions.Options;
using ThreadPilot.Insurances.Domain;

namespace ThreadPilot.Insurances.Infrastructure.Providers;

public class StubInsuranceProvider : IInsuranceProvider
{
    private readonly StubInsuranceOptions options;

    public StubInsuranceProvider(IOptions<StubInsuranceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        this.options = options.Value;
    }

    public async Task<IList<Insurance>> GetInsurancesByPersonalIdAsync(string personalId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(personalId);

        // Check for DI-seeded scenarios
        if (options.Scenarios.TryGetValue(personalId, out var scenario))
        {
            switch (scenario.Type)
            {
                case ScenarioType.Timeout:
                    await Task.Delay(TimeSpan.FromMilliseconds(scenario.DelayMs ?? options.TimeoutDelayMs),
                        cancellationToken).ConfigureAwait(false);
                    throw new TimeoutException(scenario.ErrorMessage ??
                                               "Simulated timeout from external insurance system");
                case ScenarioType.Error:
                    throw new InvalidOperationException(scenario.ErrorMessage ??
                                                        "Simulated error from external insurance system");
                case ScenarioType.Slow:
                    await Task.Delay(TimeSpan.FromMilliseconds(scenario.DelayMs ?? options.SlowDelayMs),
                        cancellationToken).ConfigureAwait(false);
                    return scenario.Payload ?? CreateDefaultInsurances();
                case ScenarioType.Empty:
                    return [];
                case ScenarioType.Success:
                    return scenario.Payload ?? CreateDefaultInsurances();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Default success scenario
        return CreateDefaultInsurances();
    }

    private static List<Insurance> CreateDefaultInsurances()
    {
        return
        [
            new Insurance(
                new Product("Home Insurance", 30.00m, "Standard terms")),

            new Insurance(
                new Product("Car Insurance", 20.00m, "Comprehensive coverage"),
                "ABC123"),

            new Insurance(
                new Product("Pet Insurance", 10.00m, "For dogs and cats"))
        ];
    }
}

public class StubInsuranceOptions
{
    public const string sectionName = "StubInsurance";

    public Dictionary<string, Scenario> Scenarios { get; } = new();
    public int TimeoutDelayMs { get; set; } = 2000;
    public int SlowDelayMs { get; set; } = 1000;
}

public record Scenario(
    ScenarioType Type,
    int? DelayMs = null,
    IList<Insurance>? Payload = null,
    string? ErrorMessage = null);

public enum ScenarioType
{
    Success,
    Empty,
    Timeout,
    Error,
    Slow
}
