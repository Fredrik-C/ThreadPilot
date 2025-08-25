using ThreadPilot.Insurances.Domain.Exceptions;
using ThreadPilot.Insurances.Domain.Validators;

namespace ThreadPilot.Insurances.Domain.ValueObjects;

public sealed record SwedishPersonalId
{
    private SwedishPersonalId(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }

    public static SwedishPersonalId Create(string input)
    {
        var result = SwedishPersonalIdValidator.Validate(input);
        if (!result.IsValid) throw new ValidationException(result.ErrorMessage ?? "Invalid Swedish personal ID.");

        return new SwedishPersonalId(input.Trim());
    }

    public static bool TryParse(string? input, out SwedishPersonalId? personalId, out string? error)
    {
        personalId = null;
        error = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Personal ID cannot be empty.";
            return false;
        }

        var result = SwedishPersonalIdValidator.Validate(input);
        if (!result.IsValid)
        {
            error = result.ErrorMessage ?? "Invalid Swedish personal ID.";
            return false;
        }

        personalId = new SwedishPersonalId(input.Trim());
        return true;
    }
}
