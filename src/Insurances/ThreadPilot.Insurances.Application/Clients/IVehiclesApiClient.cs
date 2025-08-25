using ThreadPilot.Insurances.Application.Models;

namespace ThreadPilot.Insurances.Application.Clients;

public interface IVehiclesApiClient
{
    Task<VehicleInfo?> GetVehicleAsync(string registrationNumber, CancellationToken cancellationToken = default);
}
