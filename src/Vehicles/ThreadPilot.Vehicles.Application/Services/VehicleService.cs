using System;
using System.Threading;
using System.Threading.Tasks;
using ThreadPilot.Vehicles.Domain;
using ThreadPilot.Vehicles.Domain.Validators;

namespace ThreadPilot.Vehicles.Application.Services;

public class VehicleService
{
    private readonly IVehicleInfoProvider _vehicleInfoProvider;

    public VehicleService(IVehicleInfoProvider vehicleInfoProvider)
    {
        _vehicleInfoProvider = vehicleInfoProvider;
    }

    public async Task<VehicleServiceResult> GetVehicleAsync(string registrationNumber, CancellationToken cancellationToken = default)
    {
        // Validate the registration number first
        var validationResult = SwedishVehicleRegistrationValidator.Validate(registrationNumber);
        if (!validationResult.IsValid)
        {
            return new VehicleServiceResult(false, null, VehicleServiceError.InvalidRegistrationNumber, validationResult.ErrorMessage);
        }

        var vehicle = await _vehicleInfoProvider.GetVehicleInfoAsync(registrationNumber, cancellationToken).ConfigureAwait(false);
        
        if (vehicle == null)
        {
            return new VehicleServiceResult(false, null, VehicleServiceError.NotFound, "Vehicle not found");
        }

        return new VehicleServiceResult(true, vehicle, VehicleServiceError.None, null);
    }
}

