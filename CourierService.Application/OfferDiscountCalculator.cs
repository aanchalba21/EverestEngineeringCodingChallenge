using CourierService.Domain;

namespace CourierService.Application;

public sealed class PackageCostCalculator : IPackageCostCalculator
{
    private readonly IReadOnlyList<OfferRule> _rules;

    public PackageCostCalculator(IEnumerable<OfferRule> rules)
    {
        _rules = rules?.ToList() ?? throw new ArgumentNullException(nameof(rules));
    }

    public CostBreakdown CalculateCost(decimal baseDeliveryCost, Package package)
    {
        if (baseDeliveryCost < 0)
            throw new ArgumentOutOfRangeException(nameof(baseDeliveryCost));

        if (package is null) throw new ArgumentNullException(nameof(package));

        var weightCost = (decimal)package.WeightKg * 10m;
        var distanceCost = (decimal)package.DistanceKm * 5m;
        var baseCost = baseDeliveryCost;

        var gross = baseCost + weightCost + distanceCost;

        var rule = _rules.FirstOrDefault(r => r.IsApplicable(package));
        var discountPercent = (decimal?)(rule?.DiscountPercentage) ?? 0m;

        var discountAmount = Math.Round(gross * (discountPercent / 100m), 0, MidpointRounding.AwayFromZero);
        var total = gross - discountAmount;

        return new CostBreakdown(baseCost, weightCost, distanceCost, discountAmount, total);
    }
}

