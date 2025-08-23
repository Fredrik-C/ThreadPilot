using Shouldly;
using ThreadPilot.Insurances.Domain.Exceptions;
using ThreadPilot.Insurances.Domain.ValueObjects;

namespace ThreadPilot.Insurances.UnitTests;

public class SwedishPersonalIdValueObjectTests
{
    [Theory]
    [InlineData("19640823-3234")]
    [InlineData("640823-3234")]
    public void Create_ShouldReturnValueObject_ForValidPersonalId(string input)
    {
        var obj = SwedishPersonalId.Create(input);
        obj.Value.ShouldBe(input.Trim());
        obj.ToString().ShouldBe(obj.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    public void Create_ShouldThrowValidationException_ForInvalidPersonalId(string input)
    {
        Should.Throw<ValidationException>(() => SwedishPersonalId.Create(input));
    }

    [Theory]
    [InlineData("19640823-3234")]
    [InlineData("640823-3234")]
    public void TryParse_ShouldReturnTrue_AndOutputObject_ForValidPersonalId(string input)
    {
        var ok = SwedishPersonalId.TryParse(input, out var obj, out var error);
        ok.ShouldBeTrue();
        obj.ShouldNotBeNull();
        obj!.Value.ShouldBe(input.Trim());
        error.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    public void TryParse_ShouldReturnFalse_AndError_ForInvalidPersonalId(string input)
    {
        var ok = SwedishPersonalId.TryParse(input, out var obj, out var error);
        ok.ShouldBeFalse();
        obj.ShouldBeNull();
        error.ShouldNotBeNullOrEmpty();
    }
}

