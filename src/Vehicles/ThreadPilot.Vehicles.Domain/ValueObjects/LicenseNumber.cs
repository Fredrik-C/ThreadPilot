using ThreadPilot.Vehicles.Domain.Exceptions;
using ThreadPilot.Vehicles.Domain.Validators;

namespace ThreadPilot.Vehicles.Domain.ValueObjects;

public sealed record LicenseNumber
{
    private LicenseNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }

    public static LicenseNumber Create(string input)
    {
        var result = SwedishVehicleRegistrationValidator.Validate(input);
        if (!result.IsValid) throw new ValidationException(result.ErrorMessage ?? "Invalid registration number.");

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
