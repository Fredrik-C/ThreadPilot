using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPilot.Insurances.Domain;

public interface IInsuranceProvider
{
    Task<IList<Insurance>> GetInsurancesByPersonalIdAsync(string personalId, CancellationToken cancellationToken = default);
}