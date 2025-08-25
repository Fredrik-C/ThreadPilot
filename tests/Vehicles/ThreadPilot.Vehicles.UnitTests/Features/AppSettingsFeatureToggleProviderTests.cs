using Microsoft.Extensions.Options;
using Shouldly;
using ThreadPilot.Vehicles.Application.Features;
using ThreadPilot.Vehicles.Infrastructure.Features;

namespace ThreadPilot.Vehicles.UnitTests.Features;

public class AppSettingsFeatureToggleProviderTests
{
    private sealed class TestOptionsMonitor(FeatureFlagsOptions initial) : IOptionsMonitor<FeatureFlagsOptions>
    {
        private FeatureFlagsOptions current = initial;
        private event Action<FeatureFlagsOptions, string?>? onChange;
        public FeatureFlagsOptions CurrentValue => current;
        public FeatureFlagsOptions Get(string? name) => current;
        public IDisposable OnChange(Action<FeatureFlagsOptions, string?> listener)
        {
            onChange += listener;
            return new Dummy(() => onChange -= listener);
        }
        public void Update(FeatureFlagsOptions next, string? name = null)
        {
            current = next;
            onChange?.Invoke(next, name);
        }
        private sealed class Dummy(Action dispose) : IDisposable { public void Dispose() => dispose(); }
    }

    [Fact]
    public void TryGet_ReturnsValueFromCurrentOptions()
    {
        var initial = new FeatureFlagsOptions { Flags = new() { ["A"] = true } };
        var monitor = new TestOptionsMonitor(initial);
        var provider = new AppSettingsFeatureToggleProvider(monitor);

        provider.TryGet("A", out var enabled).ShouldBeTrue();
        enabled.ShouldBeTrue();
    }

    [Fact]
    public void OnChange_UpdatesFlags()
    {
        var initial = new FeatureFlagsOptions { Flags = new() { ["A"] = false } };
        var monitor = new TestOptionsMonitor(initial);
        var provider = new AppSettingsFeatureToggleProvider(monitor);

        monitor.Update(new FeatureFlagsOptions { Flags = new() { ["A"] = true, ["B"] = true } });

        provider.TryGet("A", out var a).ShouldBeTrue();
        a.ShouldBeTrue();
        provider.TryGet("B", out var b).ShouldBeTrue();
        b.ShouldBeTrue();
        provider.TryGet("Missing", out _).ShouldBeFalse();
    }
}

