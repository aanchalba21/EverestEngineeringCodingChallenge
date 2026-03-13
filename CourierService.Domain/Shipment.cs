namespace CourierService.Domain;

public sealed class Shipment
{
    public IReadOnlyList<Package> Packages { get; }

    public int PackageCount => Packages.Count;

    public double TotalWeightKg { get; }

    public double MaxDistanceKm { get; }

    public Shipment(IEnumerable<Package> packages)
    {
        var packageList = packages.ToList();
        if (packageList.Count == 0)
            throw new ArgumentException("Shipment must contain at least one package.", nameof(packages));

        Packages = packageList;
        TotalWeightKg = packageList.Sum(p => p.WeightKg);
        MaxDistanceKm = packageList.Max(p => p.DistanceKm);
    }
}

