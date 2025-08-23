using Microsoft.Extensions.Logging;
using ThreadPilot.Insurances.Application.Clients;
using VehicleInfo = ThreadPilot.Insurances.Application.Models.VehicleInfo;
using ThreadPilot.Insurances.Domain;
using ThreadPilot.Insurances.Application.Contracts;

namespace ThreadPilot.Insurances.Application.Services;

public class InsuranceService(
    IInsuranceProvider insuranceProvider,
    IVehiclesApiClient vehiclesApiClient,
    ILogger<InsuranceService> logger)
{
    public async Task<InsuranceServiceResult> GetInsurancesAsync(
        string personalId,
        CancellationToken cancellationToken = default)
    {

        IList<Insurance> insurances;
        try
        {
            insurances = await insuranceProvider.GetInsurancesByPersonalIdAsync(personalId, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (TimeoutException ex)
        {
            logger.LogWarning(ex, "Timeout while retrieving insurances for personal ID {PersonalId}", personalId);
            return new InsuranceServiceResult(false, null, InsuranceServiceError.Timeout, "Timeout while retrieving insurances");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while retrieving insurances for personal ID {PersonalId}", personalId);
            return new InsuranceServiceResult(false, null, InsuranceServiceError.InternalError, "Error while retrieving insurances");
        }

        // Parallel enrichment for car insurances
        var tasks = new List<Task<(Insurance Insurance, VehicleInfo? Vehicle)>>();
        foreach (var insurance in insurances)
        {
            tasks.Add(!string.IsNullOrWhiteSpace(insurance.VehicleRegNo)
                ? EnrichAsync(insurance, insurance.VehicleRegNo!, cancellationToken)
                : Task.FromResult((insurance, (VehicleInfo?)null)));
        }

        var results = tasks.Count > 0
            ? await Task.WhenAll(tasks).ConfigureAwait(false)
            : [];

        var enriched = results
            .Select(tuple => new EnrichedInsurance(tuple.Insurance.Product, tuple.Insurance.VehicleRegNo, tuple.Vehicle))
            .ToList();

        return new InsuranceServiceResult(true, enriched, InsuranceServiceError.None, null, IsPartial: false);
    }

    private async Task<(Insurance Insurance, VehicleInfo? Vehicle)> EnrichAsync(Insurance insurance, string reg, CancellationToken ct)
    {
        try
        {
            var vehicle = await vehiclesApiClient.GetVehicleAsync(reg, ct).ConfigureAwait(false);
            return (insurance, vehicle);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // Swallow errors as partial enrichment per phase-04; already logged at provider layer
            return (insurance, null);
        }
    }
}
