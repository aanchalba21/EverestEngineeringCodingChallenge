using CourierService.Application;
using CourierService.Domain;

namespace CourierService.Tests;

public class ShipmentSelectorTests
{
    [Fact]
    public void SelectsShipment_MaximizingPackageCount_ThenWeight_ThenEarliestDelivery()
    {
        var packages = new[]
        {
            new Package("PKG1", 50, 30, "NA"),
            new Package("PKG2", 50, 40, "NA"),
            new Package("PKG3", 70, 10, "NA")
        };
        var vehicle = new Vehicle(1, 70, 100);

        var selector = new ShipmentSelector();
        var shipment = selector.SelectBestShipment(packages, vehicle);

        // Best is PKG1 + PKG2 (2 packages, weight 100) rather than any single pkg
        Assert.Equal(2, shipment.Packages.Count);
        Assert.Contains(shipment.Packages, p => p.Id == "PKG1");
        Assert.Contains(shipment.Packages, p => p.Id == "PKG2");
    }

    [Fact]
    public void BreaksTiesByTotalWeight()
    {
        var packages = new[]
        {
            new Package("PKG1", 30, 30, "NA"),
            new Package("PKG2", 40, 20, "NA"),
            new Package("PKG3", 50, 10, "NA")
        };

        var vehicle = new Vehicle(1, 70, 70);
        var selector = new ShipmentSelector();

        var shipment = selector.SelectBestShipment(packages, vehicle);

        // Both {PKG1, PKG2} and {PKG3, ...} may not fit, so best is PKG2 + PKG3? Actually only PKG2+PKG3 overweight.
        // With capacity 70, best two-package sets: {PKG1, PKG2} weight 70, {PKG1, PKG3} overweight.
        Assert.Equal(2, shipment.Packages.Count);
        Assert.Contains(shipment.Packages, p => p.Id == "PKG1");
        Assert.Contains(shipment.Packages, p => p.Id == "PKG2");
    }

    [Fact]
    public void BreaksTiesByEarliestDeliverable_MaxDistance()
    {
        var packages = new[]
        {
            new Package("PKG1", 30, 60, "NA"),
            new Package("PKG2", 30, 40, "NA"),
            new Package("PKG3", 30, 20, "NA")
        };

        var vehicle = new Vehicle(1, 70, 60);
        var selector = new ShipmentSelector();

        var shipment = selector.SelectBestShipment(packages, vehicle);

        // Two-package combinations all weigh 60, so choose the pair with smallest max distance: PKG2 + PKG3 (max distance 40)
        Assert.Equal(2, shipment.Packages.Count);
        Assert.Contains(shipment.Packages, p => p.Id == "PKG2");
        Assert.Contains(shipment.Packages, p => p.Id == "PKG3");
    }
}

