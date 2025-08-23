namespace ThreadPilot.Vehicles.Application.Features;

public interface IFeatureToggle
{
    bool IsEnabled(string featureName, bool @default = false);
}

public interface IFeatureToggle<TEnum> where TEnum : struct, System.Enum
{
    bool IsEnabled(TEnum feature, bool @default = false);
}

public interface IFeatureToggleProvider
{
    bool TryGet(string featureName, out bool isEnabled);
}

