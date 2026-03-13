using CourierService.Domain;

namespace CourierService.Application;

public sealed class CostEstimationResult
{
    public Package Package { get; }
    public decimal DiscountAmount { get; }
    public decimal TotalCost { get; }

    public CostEstimationResult(Package package, decimal discountAmount, decimal totalCost)
    {
        Package = package ?? throw new ArgumentNullException(nameof(package));
        DiscountAmount = discountAmount;
        TotalCost = totalCost;
    }
}

