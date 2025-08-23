using Microsoft.Extensions.Options;
using Shouldly;
using ThreadPilot.Insurances.Application.Features;
using ThreadPilot.Insurances.Infrastructure.Features;

namespace ThreadPilot.Insurances.UnitTests.Features;

public class AppSettingsFeatureToggleProviderTests
{
    private sealed class TestOptionsMonitor : IOptionsMonitor<FeatureFlagsOptions>
    {
        private FeatureFlagsOptions _current;
        private event Action<FeatureFlagsOptions, string?>? _onChange;
        public TestOptionsMonitor(FeatureFlagsOptions initial) => _current = initial;
        public FeatureFlagsOptions CurrentValue => _current;
        public FeatureFlagsOptions Get(string? name) => _current;
        public IDisposable OnChange(Action<FeatureFlagsOptions, string?> listener)
        {
            _onChange += listener;
            return new Dummy(() => _onChange -= listener);
        }
        public void Update(FeatureFlagsOptions next, string? name = null)
        {
            _current = next;
            _onChange?.Invoke(next, name);
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

