namespace CourierService.Domain;

public sealed class OfferRule
{
    public string Code { get; }
    public double DiscountPercentage { get; }
    public double MinDistanceKm { get; }
    public double MaxDistanceKm { get; }
    public double MinWeightKg { get; }
    public double MaxWeightKg { get; }

    public OfferRule(
        string code,
        double discountPercentage,
        double minDistanceKm,
        double maxDistanceKm,
        double minWeightKg,
        double maxWeightKg)
    {
        Code = code.Trim().ToUpperInvariant();
        DiscountPercentage = discountPercentage;
        MinDistanceKm = minDistanceKm;
        MaxDistanceKm = maxDistanceKm;
        MinWeightKg = minWeightKg;
        MaxWeightKg = maxWeightKg;
    }

    public bool IsApplicable(Package package)
    {
        if (!string.Equals(package.OfferCode, Code, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return package.DistanceKm >= MinDistanceKm &&
               package.DistanceKm <= MaxDistanceKm &&
               package.WeightKg >= MinWeightKg &&
               package.WeightKg <= MaxWeightKg;
    }
}

