using CourierService.Application;
using CourierService.Domain;

namespace CourierService.Tests;

public class DeliveryTimeEstimatorTests
{
    [Fact]
    public void SchedulesShipments_ByEarliestVehicleAvailability()
    {
        var rules = Array.Empty<OfferRule>();
        var costCalculator = new PackageCostCalculator(rules);
        var costService = new CostEstimationService(costCalculator);

        var pkg1 = new Package("PKG1", 50, 30, "NA");
        var pkg2 = new Package("PKG2", 50, 60, "NA");

        var costResults = new[]
        {
            costService.EstimateCost(100m, pkg1),
            costService.EstimateCost(100m, pkg2)
        };

        var selector = new ShipmentSelector();
        var estimator = new DeliveryTimeEstimator(selector);

        var times = estimator.EstimateDeliveryTimes(costResults, numberOfVehicles: 1, maxSpeedKmph: 60, maxCarriableWeightKg: 100);

        // first shipment contains both packages: max distance 60km -> one way 1h
        Assert.Equal(1.0, times["PKG1"], 2);
        Assert.Equal(1.0, times["PKG2"], 2);
    }
}

