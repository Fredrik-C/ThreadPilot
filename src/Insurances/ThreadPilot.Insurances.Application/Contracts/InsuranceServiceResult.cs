namespace ThreadPilot.Insurances.Application.Contracts;

public enum InsuranceServiceError
{
    None,
    InvalidPersonalId,
    NotFound,
    Timeout,
    InternalError
}

public record InsuranceServiceResult(
    bool IsSuccess,
    IList<EnrichedInsurance>? Insurances,
    InsuranceServiceError Error,
    string? ErrorMessage,
    bool IsPartial = false);

