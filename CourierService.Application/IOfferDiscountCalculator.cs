using CourierService.Domain;

namespace CourierService.Application;

public interface IPackageCostCalculator
{
    CostBreakdown CalculateCost(decimal baseDeliveryCost, Package package);
}

