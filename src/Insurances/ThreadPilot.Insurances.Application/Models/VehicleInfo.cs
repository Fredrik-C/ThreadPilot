namespace ThreadPilot.Insurances.Application.Models;

public sealed record VehicleInfo(
    string RegNo,
    string Make,
    string Model,
    int Year,
    string FuelType);
