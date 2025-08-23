using ThreadPilot.Vehicles.Domain;

namespace ThreadPilot.Vehicles.Application.Services;

public record VehicleServiceResult(
    bool IsSuccess,
    Vehicle? Vehicle,
    VehicleServiceError Error,
    string? ErrorMessage);

