using ThreadPilot.Insurances.Application.Models;
using ThreadPilot.Insurances.Domain;

namespace ThreadPilot.Insurances.Application.Contracts;

public sealed record EnrichedInsurance(
    Product Product,
    string? VehicleRegNo,
    VehicleInfo? Vehicle);

