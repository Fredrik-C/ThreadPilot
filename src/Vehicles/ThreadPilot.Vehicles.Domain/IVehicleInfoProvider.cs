namespace ThreadPilot.Vehicles.Domain;

public interface IVehicleInfoProvider
{
    Task<Vehicle?> GetVehicleInfoAsync(string registrationNumber, CancellationToken cancellationToken = default);
}
