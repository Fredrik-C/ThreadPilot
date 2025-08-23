using ThreadPilot.Vehicles.Application.Features;

namespace ThreadPilot.Vehicles.Infrastructure.Features;

public sealed class CompositeFeatureToggle : IFeatureToggle
{
    private readonly IReadOnlyList<IFeatureToggleProvider> _providers;

    public CompositeFeatureToggle(IEnumerable<IFeatureToggleProvider> providers)
    {
        _providers = providers.ToList();
    }

    public bool IsEnabled(string featureName, bool @default = false)
    {
        foreach (var p in _providers)
        {
            if (p.TryGet(featureName, out var value))
                return value;
        }
        return @default;
    }
}

