using System.Diagnostics.CodeAnalysis;
using ThreadPilot.Insurances.Domain;

namespace ThreadPilot.Insurances.Api.DTOs;

[SuppressMessage("Design", "CA1515", Justification = "Public DTO used in action signatures and OpenAPI")]
public sealed record ProductDto(
    string Name,
    decimal Price,
    string Terms)
{
    public static ProductDto FromDomain(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        return new ProductDto(
            product.Name,
            product.Price,
            product.Terms);
    }
}
