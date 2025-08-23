using ThreadPilot.Vehicles.Domain;
using ThreadPilot.Vehicles.Domain.Validators;
using ThreadPilot.Vehicles.Application.Contracts;

namespace ThreadPilot.Vehicles.Application.Services;

public class VehicleService(IVehicleInfoProvider vehicleInfoProvider)
{
    public async Task<VehicleServiceResult> GetVehicleAsync(string registrationNumber, CancellationToken cancellationToken = default)
    {
        // Validate the registration number first
        var validationResult = SwedishVehicleRegistrationValidator.Validate(registrationNumber);
        if (!validationResult.IsValid)
        {
            return new VehicleServiceResult(false, null, VehicleServiceError.InvalidRegistrationNumber, validationResult.ErrorMessage);
        }

        var vehicle = await vehicleInfoProvider.GetVehicleInfoAsync(registrationNumber, cancellationToken).ConfigureAwait(false);

        return vehicle == null ? new VehicleServiceResult(false, null, VehicleServiceError.NotFound, "Vehicle not found") : new VehicleServiceResult(true, vehicle, VehicleServiceError.None, null);
    }
}

