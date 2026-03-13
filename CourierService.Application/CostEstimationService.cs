using CourierService.Domain;

namespace CourierService.Application;

public sealed class CostEstimationService
{
    private readonly IPackageCostCalculator _costCalculator;

    public CostEstimationService(IPackageCostCalculator costCalculator)
    {
        _costCalculator = costCalculator ?? throw new ArgumentNullException(nameof(costCalculator));
    }

    public CostEstimationResult EstimateCost(decimal baseDeliveryCost, Package package)
    {
        var breakdown = _costCalculator.CalculateCost(baseDeliveryCost, package);
        return new CostEstimationResult(package, breakdown.DiscountAmount, breakdown.TotalCost);
    }
}

