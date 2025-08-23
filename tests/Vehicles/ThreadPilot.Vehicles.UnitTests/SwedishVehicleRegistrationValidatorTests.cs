using Shouldly;
using ThreadPilot.Vehicles.Domain.Validators;

namespace ThreadPilot.Vehicles.UnitTests;

public class SwedishVehicleRegistrationValidatorTests
{
    [Theory]
    [InlineData("ABC123")]
    [InlineData("ABC 123")]
    [InlineData("DEF45G")]
    [InlineData("DEF 45G")]
    [InlineData("XYZ789")]
    [InlineData("XYZ 789")]
    [InlineData("AB123C")]
    [InlineData("AB 123C")]
    public void IsValid_ShouldReturnTrue_ForStandardRegistrationNumbers(string registrationNumber)
    {
        SwedishVehicleRegistrationValidator.IsValid(registrationNumber).ShouldBeTrue();
    }

    [Theory]
    [InlineData("Volvo")]
    [InlineData("Volvo1")]
    [InlineData("Volvo12")]
    public void IsValid_ShouldReturnTrue_ForPersonalRegistrationNumbers(string registrationNumber)
    {
        SwedishVehicleRegistrationValidator.IsValid(registrationNumber).ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Volvo123")]
    [InlineData("Volvo1234")]
    [InlineData("Volvo12345")]
    [InlineData("Volvo123456")]
    [InlineData("Volvo1234567")]
    [InlineData("Volvo12345678")]
    public void IsValid_ShouldReturnFalse_ForInvalidRegistrationNumbers(string registrationNumber)
    {
        SwedishVehicleRegistrationValidator.IsValid(registrationNumber).ShouldBeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnValidResult_ForValidRegistrationNumber()
    {
        var result = SwedishVehicleRegistrationValidator.Validate("ABC123");
        result.IsValid.ShouldBeTrue();
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void Validate_ShouldReturnInvalidResult_ForInvalidRegistrationNumber()
    {
        var result = SwedishVehicleRegistrationValidator.Validate("");
        result.IsValid.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNullOrEmpty();
    }
}
