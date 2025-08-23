using Microsoft.AspNetCore.Mvc;

namespace ThreadPilot.Vehicles.Api;

internal static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(this Application.Services.VehicleServiceResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.Error switch
        {
            Application.Services.VehicleServiceError.InvalidRegistrationNumber => new ProblemDetails
            {
                Title = "Invalid Registration Number",
                Detail = result.ErrorMessage,
                Status = 400,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            Application.Services.VehicleServiceError.NotFound => new ProblemDetails
            {
                Title = "Vehicle Not Found",
                Detail = result.ErrorMessage,
                Status = 404,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },
            Application.Services.VehicleServiceError.Timeout => new ProblemDetails
            {
                Title = "Service Unavailable",
                Detail = result.ErrorMessage,
                Status = 503,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4"
            },
            Application.Services.VehicleServiceError.InternalError => new ProblemDetails
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
    }
}