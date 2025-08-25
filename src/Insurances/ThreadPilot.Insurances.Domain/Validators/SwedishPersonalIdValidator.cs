using System.Globalization;
using System.Text.RegularExpressions;

namespace ThreadPilot.Insurances.Domain.Validators;

public static class SwedishPersonalIdValidator
{
    // Support both 10-digit (YYMMDD-XXXX) and 12-digit (YYYYMMDD-XXXX) formats
    private static readonly Regex regex = new(
        @"^(\d{2})?(\d{6})-(\d{4})$",
        RegexOptions.IgnoreCase
    );

    public static bool IsValid(string personalId)
    {
        if (string.IsNullOrWhiteSpace(personalId))
            return false;

        var match = regex.Match(personalId.Trim());
        if (!match.Success)
            return false;

        var fullDigits = match.Groups[1].Value + match.Groups[2].Value + match.Groups[3].Value;
        return ValidateChecksum(fullDigits);
    }

    public static ValidationResult Validate(string personalId)
    {
        if (string.IsNullOrWhiteSpace(personalId)) return new ValidationResult(false, "Personal ID cannot be empty.");

        var trimmed = personalId.Trim();
        var match = regex.Match(trimmed);
        if (!match.Success)
        {
            return new ValidationResult(false,
                "Invalid Swedish personal ID format. Expected YYMMDD-XXXX or YYYYMMDD-XXXX.");
        }

        var fullDigits = match.Groups[1].Value + match.Groups[2].Value + match.Groups[3].Value;
        return !ValidateChecksum(fullDigits)
            ? new ValidationResult(false, "Invalid Swedish personal ID checksum.")
            : new ValidationResult(true, null);
    }

    private static bool ValidateChecksum(string digits)
    {
        if (digits.Length is not 10 and not 12)
            return false;

        // If 12 digits, take the last 10
        if (digits.Length == 12)
            digits = digits[2..];

        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            var digit = int.Parse(digits[i].ToString(), CultureInfo.InvariantCulture);
            // Double every second digit from right, but not the first (checksum)
            // From right to left, positions are 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            // We double positions 9, 7, 5, 3, 1 (from right)
            // Which means we double indices 0, 2, 4, 6, 8 (from left)
            if (i % 2 == 0)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
        }

        var checksum = (10 - sum % 10) % 10;
        var expectedChecksum = int.Parse(digits[9].ToString(), CultureInfo.InvariantCulture);

        return checksum == expectedChecksum;
    }
}

public record ValidationResult(bool IsValid, string? ErrorMessage);
