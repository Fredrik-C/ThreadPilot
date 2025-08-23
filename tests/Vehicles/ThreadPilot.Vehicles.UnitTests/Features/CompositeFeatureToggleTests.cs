using Moq;
using Shouldly;
using ThreadPilot.Vehicles.Application.Features;
using ThreadPilot.Vehicles.Infrastructure.Features;

namespace ThreadPilot.Vehicles.UnitTests.Features;

public class CompositeFeatureToggleTests
{
    [Fact]
    public void IsEnabled_UsesFirstProviderThatHasValue()
    {
        var p1 = new Mock<IFeatureToggleProvider>();
        var p2 = new Mock<IFeatureToggleProvider>();
        p1.Setup(x => x.TryGet("X", out It.Ref<bool>.IsAny))
          .Returns((string _, out bool v) => { v = false; return true; });
        p2.Setup(x => x.TryGet("X", out It.Ref<bool>.IsAny))
          .Returns((string _, out bool v) => { v = true; return true; });

        var sut = new CompositeFeatureToggle(new[] { p1.Object, p2.Object });
        sut.IsEnabled("X", @default: true).ShouldBeFalse();
    }

    [Fact]
    public void IsEnabled_ReturnsDefaultWhenNoProviderHasValue()
    {
        var p1 = new Mock<IFeatureToggleProvider>();
        var p2 = new Mock<IFeatureToggleProvider>();
        p1.Setup(x => x.TryGet("Y", out It.Ref<bool>.IsAny))
          .Returns((string _, out bool v) => { v = false; return false; });
        p2.Setup(x => x.TryGet("Y", out It.Ref<bool>.IsAny))
          .Returns((string _, out bool v) => { v = true; return false; });

        var sut = new CompositeFeatureToggle(new[] { p1.Object, p2.Object });
        sut.IsEnabled("Y", @default: true).ShouldBeTrue();
        sut.IsEnabled("Y", @default: false).ShouldBeFalse();
    }
}

