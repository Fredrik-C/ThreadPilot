using Microsoft.Extensions.Options;
using ThreadPilot.Vehicles.Application.Features;

namespace ThreadPilot.Vehicles.Infrastructure.Features;

public sealed class AppSettingsFeatureToggleProvider : IFeatureToggleProvider
{
    private volatile IReadOnlyDictionary<string, bool> _flags;

    public AppSettingsFeatureToggleProvider(IOptionsMonitor<FeatureFlagsOptions> monitor)
    {
        _flags = monitor.CurrentValue.Flags;
        monitor.OnChange(o => _flags = o.Flags ?? new Dictionary<string, bool>());
    }

    public bool TryGet(string featureName, out bool isEnabled)
        => _flags.TryGetValue(featureName, out isEnabled);
}

