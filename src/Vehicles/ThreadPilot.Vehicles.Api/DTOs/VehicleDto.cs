using System.Diagnostics.CodeAnalysis;
namespace ThreadPilot.Vehicles.Api.DTOs;

[SuppressMessage("Design", "CA1515", Justification = "Public DTO used in action signatures and OpenAPI")]
public sealed record VehicleDto(
    string RegNo,
    string Make,
    string Model,
    int Year,
    string FuelType)
{
    public static VehicleDto FromDomain(Domain.Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);

        return new VehicleDto(
            vehicle.RegNo,
            vehicle.Make,
            vehicle.Model,
            vehicle.Year,
            vehicle.FuelType);
    }
}