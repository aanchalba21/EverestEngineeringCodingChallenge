using CourierService.Application;
using CourierService.Domain;

namespace CourierService.Tests;

public class CostAndDiscountTests
{
    private static PackageCostCalculator CreateCalculator()
    {
        var rules = new[]
        {
            new OfferRule("OFR001", 10, 0, 200, 70, 200),
            new OfferRule("OFR002", 7, 50, 150, 100, 250),
            new OfferRule("OFR003", 5, 50, 250, 10, 150)
        };
        return new PackageCostCalculator(rules);
    }

    [Fact]
    public void AppliesValidDiscount_WhenOfferAndConstraintsMatch()
    {
        var calculator = CreateCalculator();
        var service = new CostEstimationService(calculator);
        var pkg = new Package("PKG1", 100, 100, "OFR002");

        var result = service.EstimateCost(100m, pkg);

        // cost without discount: 100 + 100*10 + 100*5 = 100 + 1000 + 500 = 1600
        Assert.Equal(112m, result.DiscountAmount); // 7% of 1600
        Assert.Equal(1488m, result.TotalCost);
    }

    [Fact]
    public void NoDiscount_WhenOfferCodeInvalid()
    {
        var calculator = CreateCalculator();
        var service = new CostEstimationService(calculator);
        var pkg = new Package("PKG1", 100, 100, "INVALID");

        var result = service.EstimateCost(100m, pkg);

        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(100m + 100m * 10m + 100m * 5m, result.TotalCost);
    }

    [Fact]
    public void NoDiscount_WhenOfferIneligibleByWeight()
    {
        var calculator = CreateCalculator();
        var service = new CostEstimationService(calculator);
        // OFR002 requires weight >=100, so 90 should be ineligible
        var pkg = new Package("PKG1", 90, 100, "OFR002");

        var result = service.EstimateCost(100m, pkg);

        Assert.Equal(0m, result.DiscountAmount);
    }
}