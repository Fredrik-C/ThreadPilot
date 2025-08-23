using System.Diagnostics.CodeAnalysis;
using ThreadPilot.Insurances.Application.Models;

namespace ThreadPilot.Insurances.Api.DTOs;

[SuppressMessage("Design", "CA1515", Justification = "Public DTO used in action signatures and OpenAPI")]
public sealed record VehicleInfoDto(
    string RegNo,
    string Make,
    string Model,
    int Year,
    string FuelType)
{
    public static VehicleInfoDto FromApplication(VehicleInfo vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);

        return new VehicleInfoDto(
            vehicle.RegNo,
            vehicle.Make,
            vehicle.Model,
            vehicle.Year,
            vehicle.FuelType);
    }
}

