namespace ThreadPilot.Insurances.Infrastructure.Features;

public sealed class FeatureFlagsOptions
{
    public const string SectionName = "Features";
    public Dictionary<string, bool> Flags { get; init; } = new();
}
