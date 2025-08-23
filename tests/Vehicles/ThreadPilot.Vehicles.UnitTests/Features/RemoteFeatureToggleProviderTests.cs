using Shouldly;
using ThreadPilot.Vehicles.Infrastructure.Features;

namespace ThreadPilot.Vehicles.UnitTests.Features;

public class RemoteFeatureToggleProviderTests
{
    [Fact]
    public void TryGet_ReturnsFalseWhenNotPresent()
    {
        var sut = new RemoteFeatureToggleProvider();
        sut.TryGet("Missing", out _).ShouldBeFalse();
    }

    [Fact]
    public void Update_AddsAndUpdatesFlags()
    {
        var sut = new RemoteFeatureToggleProvider();
        sut.Update(new Dictionary<string, bool> { ["A"] = true });
        sut.TryGet("A", out var a).ShouldBeTrue();
        a.ShouldBeTrue();

        sut.Update(new Dictionary<string, bool> { ["A"] = false, ["B"] = true });
        sut.TryGet("A", out var a2).ShouldBeTrue();
        a2.ShouldBeFalse();
        sut.TryGet("B", out var b).ShouldBeTrue();
        b.ShouldBeTrue();
    }
}

