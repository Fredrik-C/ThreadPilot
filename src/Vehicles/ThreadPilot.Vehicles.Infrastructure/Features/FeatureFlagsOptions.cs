namespace ThreadPilot.Vehicles.Infrastructure.Features;

public sealed class FeatureFlagsOptions
{
    public const string SectionName = "Features";
    public Dictionary<string, bool> Flags { get; init; } = new();
}
