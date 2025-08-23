using System.Threading;
using System.Threading.Tasks;

namespace ThreadPilot.Vehicles.Domain;

public interface IVehicleInfoProvider
{
    Task<Vehicle?> GetVehicleInfoAsync(string registrationNumber, CancellationToken cancellationToken = default);
}