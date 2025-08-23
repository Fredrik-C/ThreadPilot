using System.Diagnostics.CodeAnalysis;
using ThreadPilot.Insurances.Application.Contracts;

namespace ThreadPilot.Insurances.Api.DTOs;

[SuppressMessage("Design", "CA1515", Justification = "Public DTO used in action signatures and OpenAPI")]
public sealed record EnrichedInsuranceDto(
    ProductDto Product,
    string? VehicleRegNo,
    VehicleInfoDto? Vehicle)
{
    public static EnrichedInsuranceDto FromApplication(EnrichedInsurance insurance)
    {
        ArgumentNullException.ThrowIfNull(insurance);

        return new EnrichedInsuranceDto(
            ProductDto.FromDomain(insurance.Product),
            insurance.VehicleRegNo,
            insurance.Vehicle is null ? null : VehicleInfoDto.FromApplication(insurance.Vehicle));
    }
}
