using System.Collections.Concurrent;
using ThreadPilot.Vehicles.Application.Features;

namespace ThreadPilot.Vehicles.Infrastructure.Features;

// Stubbed remote provider; to be updated with polling and HTTP fetching in later phases
public sealed class RemoteFeatureToggleProvider : IFeatureToggleProvider
{
    private readonly ConcurrentDictionary<string, bool> _cache = new();

    public bool TryGet(string featureName, out bool isEnabled)
    {
        return _cache.TryGetValue(featureName, out isEnabled);
    }

    // Internal update method for future BackgroundService
    public void Update(IDictionary<string, bool> flags)
    {
        foreach (var kv in flags) _cache[kv.Key] = kv.Value;
    }
}
