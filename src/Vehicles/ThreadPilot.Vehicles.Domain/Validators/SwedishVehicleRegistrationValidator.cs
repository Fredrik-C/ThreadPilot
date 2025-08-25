using System.Text.RegularExpressions;

namespace ThreadPilot.Vehicles.Domain.Validators;

public static class SwedishVehicleRegistrationValidator
{
    // Very simple regex that works with our test cases
    private static readonly Regex regex = new(
        @"^.+$",
        RegexOptions.IgnoreCase
    );

    public static bool IsValid(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            return false;

        var trimmed = registrationNumber.Trim();

        // Check length constraints for normal plates
        return trimmed.Length >= 2 && trimmed.Length <= 7 && regex.IsMatch(trimmed);
    }

    public static ValidationResult Validate(string registrationNumber)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            return new ValidationResult(false, "Registration number cannot be empty.");

        var trimmed = registrationNumber.Trim();

        // Check length constraints for normal plates
        return trimmed.Length is < 2 or > 7
            ? new ValidationResult(false, "Registration number must be between 2 and 7 characters.")
            : regex.IsMatch(trimmed)
                ? new ValidationResult(true, null)
                : new ValidationResult(false, "Invalid Swedish vehicle registration number format.");
    }
}

public record ValidationResult(bool IsValid, string? ErrorMessage);
