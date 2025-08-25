using Microsoft.AspNetCore.Mvc;
using ThreadPilot.Vehicles.Application.Contracts;

namespace ThreadPilot.Vehicles.Api;

internal static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this VehicleServiceResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var problemDetails = result.Error switch
        {
            VehicleServiceError.InvalidRegistrationNumber => new ProblemDetails
            {
                Title = "Invalid Registration Number",
                Detail = result.ErrorMessage,
                Status = 400,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            VehicleServiceError.NotFound => new ProblemDetails
            {
                Title = "Vehicle Not Found",
                Detail = result.ErrorMessage,
                Status = 404,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },
            VehicleServiceError.Timeout => new ProblemDetails
            {
                Title = "Service Unavailable",
                Detail = result.ErrorMessage,
                Status = 503,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4"
            },
            VehicleServiceError.InternalError => new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = result.ErrorMessage,
                Status = 500,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            },
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
