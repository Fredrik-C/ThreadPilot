using Shouldly;
using ThreadPilot.Vehicles.Domain.Exceptions;
using ThreadPilot.Vehicles.Domain.ValueObjects;

namespace ThreadPilot.Vehicles.UnitTests;

public class LicenseNumberValueObjectTests
{
    [Theory]
    [InlineData("ABC123")]
    [InlineData("XYZ 789")]

    public void Create_ShouldReturnValueObject_ForValidRegistrationNumber(string input)
    {
        var obj = LicenseNumber.Create(input);
        obj.Value.ShouldBe(input.Trim());
        obj.ToString().ShouldBe(obj.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Volvo12345678")]
    public void Create_ShouldThrowValidationException_ForInvalidRegistrationNumber(string input)
    {
        Should.Throw<ValidationException>(() => LicenseNumber.Create(input));
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("XYZ 789")]
    public void TryParse_ShouldReturnTrue_AndOutputObject_ForValidRegistrationNumber(string input)
    {
        var ok = LicenseNumber.TryParse(input, out var obj, out var error);
        ok.ShouldBeTrue();
        obj.ShouldNotBeNull();
        obj!.Value.ShouldBe(input.Trim());
        error.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Volvo12345678")]
    public void TryParse_ShouldReturnFalse_AndError_ForInvalidRegistrationNumber(string input)
    {
        var ok = LicenseNumber.TryParse(input, out var obj, out var error);
        ok.ShouldBeFalse();
        obj.ShouldBeNull();
        error.ShouldNotBeNullOrEmpty();
    }
}

