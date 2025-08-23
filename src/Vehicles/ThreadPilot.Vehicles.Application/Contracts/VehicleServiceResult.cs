using ThreadPilot.Vehicles.Domain;

namespace ThreadPilot.Vehicles.Application.Contracts;

public enum VehicleServiceError
{
    None,
    InvalidRegistrationNumber,
    NotFound,
    Timeout,
    InternalError
}

public record VehicleServiceResult(
    bool IsSuccess,
    Vehicle? Vehicle,
    VehicleServiceError Error,
    string? ErrorMessage);

