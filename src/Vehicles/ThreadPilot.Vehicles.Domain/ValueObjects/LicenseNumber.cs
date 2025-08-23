using ThreadPilot.Vehicles.Domain.Validators;
using ThreadPilot.Vehicles.Domain.Exceptions;

namespace ThreadPilot.Vehicles.Domain.ValueObjects;

public sealed record LicenseNumber
{
    public string Value { get; }

    private LicenseNumber(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;

    public static LicenseNumber Create(string input)
    {
        var result = SwedishVehicleRegistrationValidator.Validate(input);
        if (!result.IsValid)
        {
            throw new ValidationException(result.ErrorMessage ?? "Invalid registration number.");
        }

        return new LicenseNumber(input.Trim());
    }

    public static bool TryParse(string? input, out LicenseNumber? license, out string? error)
    {
        license = null;
        error = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Registration number cannot be empty.";
            return false;
        }

        var result = SwedishVehicleRegistrationValidator.Validate(input);
        if (!result.IsValid)
        {
            error = result.ErrorMessage ?? "Invalid registration number.";
            return false;
        }

        license = new LicenseNumber(input.Trim());
        return true;
    }
}

