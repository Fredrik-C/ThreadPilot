namespace ThreadPilot.Vehicles.Domain;

public record Vehicle(
    string RegNo,
    string Make,
    string Model,
    int Year,
    string FuelType);
