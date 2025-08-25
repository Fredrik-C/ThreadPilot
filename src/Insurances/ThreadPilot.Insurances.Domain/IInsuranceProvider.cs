namespace ThreadPilot.Insurances.Domain;

public interface IInsuranceProvider
{
    Task<IList<Insurance>> GetInsurancesByPersonalIdAsync(string personalId,
        CancellationToken cancellationToken = default);
}
