namespace CourierService.Domain;

public sealed class Package
{
    public string Id { get; }
    public double WeightKg { get; }
    public double DistanceKm { get; }
    public string OfferCode { get; }

    public Package(string id, double weightKg, double distanceKm, string offerCode)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Package id is required.", nameof(id));

        if (weightKg <= 0)
            throw new ArgumentOutOfRangeException(nameof(weightKg), "Weight must be positive.");

        if (distanceKm < 0)
            throw new ArgumentOutOfRangeException(nameof(distanceKm), "Distance cannot be negative.");

        Id = id.Trim();
        WeightKg = weightKg;
        DistanceKm = distanceKm;
        OfferCode = offerCode?.Trim().ToUpperInvariant() ?? string.Empty;
    }
}

