using CourierService.Domain;

namespace CourierService.ConsoleApp;

public sealed class ProblemInput
{
    public decimal BaseDeliveryCost { get; }
    public IReadOnlyList<Package> Packages { get; }
    public int? NumberOfVehicles { get; }
    public double? MaxSpeedKmph { get; }
    public double? MaxCarriableWeightKg { get; }

    public ProblemInput(
        decimal baseDeliveryCost,
        IReadOnlyList<Package> packages,
        int? numberOfVehicles,
        double? maxSpeedKmph,
        double? maxCarriableWeightKg)
    {
        BaseDeliveryCost = baseDeliveryCost;
        Packages = packages;
        NumberOfVehicles = numberOfVehicles;
        MaxSpeedKmph = maxSpeedKmph;
        MaxCarriableWeightKg = maxCarriableWeightKg;
    }
}

