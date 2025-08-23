using Shouldly;
using ThreadPilot.Insurances.Domain.Validators;

namespace ThreadPilot.Insurances.UnitTests;

public class SwedishPersonalIdValidatorTests
{
    [Theory]
    [InlineData("19640823-3234")] // Valid 12-digit ID
    [InlineData("640823-3234")]   // Valid 10-digit ID
    public void IsValid_ShouldReturnTrue_ForValidPersonalIds(string personalId)
    {
        SwedishPersonalIdValidator.IsValid(personalId).ShouldBeTrue();
    }

    [Theory]
    [InlineData("19900101-1235")] // Invalid checksum
    [InlineData("900101-1235")]   // Invalid checksum
    [InlineData("199001011234")]  // Missing dash
    [InlineData("9001011234")]    // Missing dash
    [InlineData("19900101-123")]  // Too short
    [InlineData("900101-123")]    // Too short
    [InlineData("19900101-12345")] // Too long
    [InlineData("900101-12345")]   // Too long
    [InlineData("")]              // Empty
    [InlineData(" ")]             // Whitespace
    [InlineData("invalid")]       // Invalid format
    public void IsValid_ShouldReturnFalse_ForInvalidPersonalIds(string personalId)
    {
        SwedishPersonalIdValidator.IsValid(personalId).ShouldBeFalse();
    }

    [Fact]
    public void Validate_ShouldReturnValidResult_ForValidPersonalId()
    {
        var result = SwedishPersonalIdValidator.Validate("19640823-3234");
        result.IsValid.ShouldBeTrue();
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void Validate_ShouldReturnInvalidResult_ForInvalidPersonalId()
    {
        var result = SwedishPersonalIdValidator.Validate("");
        result.IsValid.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNullOrEmpty();
    }
}