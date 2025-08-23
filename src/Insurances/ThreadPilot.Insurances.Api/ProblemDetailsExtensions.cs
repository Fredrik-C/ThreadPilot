using Microsoft.AspNetCore.Mvc;

namespace ThreadPilot.Insurances.Api;

internal static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this Application.Contracts.InsuranceServiceResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var problemDetails = result.Error switch
        {
            Application.Contracts.InsuranceServiceError.InvalidPersonalId => new ProblemDetails
            {
                Title = "Invalid Personal ID",
                Detail = result.ErrorMessage,
                Status = 400,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            Application.Contracts.InsuranceServiceError.NotFound => new ProblemDetails
            {
                Title = "Insurances Not Found",
                Detail = result.ErrorMessage,
                Status = 404,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },
            Application.Contracts.InsuranceServiceError.Timeout => new ProblemDetails
            {
                Title = "Service Unavailable",
                Detail = result.ErrorMessage,
                Status = 503,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4"
            },
            Application.Contracts.InsuranceServiceError.InternalError => new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = result.ErrorMessage,
                Status = 500,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            },
            Application.Contracts.InsuranceServiceError.None => throw new NotImplementedException(),
            _ => new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred",
                Status = 500,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };

        // Add stable machine-readable error code for client logic
        problemDetails.Extensions["code"] = result.Error.ToString();
        return problemDetails;
    }
}