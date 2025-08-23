using System.Diagnostics.CodeAnalysis;
using ThreadPilot.Insurances.Domain;

namespace ThreadPilot.Insurances.Api.DTOs;

[SuppressMessage("Design", "CA1515", Justification = "Public DTO used in action signatures and OpenAPI")]
public sealed record InsuranceDto(
    ProductDto Product,
    string? VehicleRegNo)
{
    public static InsuranceDto FromDomain(Insurance insurance)
    {
        ArgumentNullException.ThrowIfNull(insurance);

        return new InsuranceDto(
            ProductDto.FromDomain(insurance.Product),
            insurance.VehicleRegNo);
    }
}