namespace ThreadPilot.Insurances.Domain;

public record Insurance(
    Product Product,
    string? VehicleRegNo = null);