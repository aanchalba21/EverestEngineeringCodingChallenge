using CourierService.Domain;

namespace CourierService.Application;

public sealed class DeliveryTimeEstimator
{
    private readonly ShipmentSelector _shipmentSelector;

    public DeliveryTimeEstimator(ShipmentSelector shipmentSelector)
    {
        _shipmentSelector = shipmentSelector ?? throw new ArgumentNullException(nameof(shipmentSelector));
    }

    public IDictionary<string, double> EstimateDeliveryTimes(
        IReadOnlyList<CostEstimationResult> costResults,
        int numberOfVehicles,
        double maxSpeedKmph,
        double maxCarriableWeightKg)
    {
        if (numberOfVehicles <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfVehicles));
        if (maxSpeedKmph <= 0) throw new ArgumentOutOfRangeException(nameof(maxSpeedKmph));
        if (maxCarriableWeightKg <= 0) throw new ArgumentOutOfRangeException(nameof(maxCarriableWeightKg));

        var packages = costResults.Select(r => r.Package).ToList();
        var vehicles = Enumerable.Range(1, numberOfVehicles)
            .Select(id => new Vehicle(id, maxSpeedKmph, maxCarriableWeightKg))
            .ToList();

        var remainingPackages = new HashSet<Package>(packages);
        var deliveryTimes = new Dictionary<string, double>();

        while (remainingPackages.Count > 0)
        {
            // choose earliest available vehicle (and smallest id for stability)
            var vehicle = vehicles
                .OrderBy(v => v.NextAvailableTimeHours)
                .ThenBy(v => v.Id)
                .First();

            var candidatePackages = remainingPackages.ToList();
            var shipment = _shipmentSelector.SelectBestShipment(candidatePackages, vehicle);

            var startTime = vehicle.NextAvailableTimeHours;
            var maxDistance = shipment.MaxDistanceKm;
            var oneWayHours = maxDistance / vehicle.MaxSpeedKmph;
            var roundTripHours = oneWayHours * 2;

            var roundedOneWay = Math.Round(oneWayHours, 2, MidpointRounding.AwayFromZero);

            foreach (var pkg in shipment.Packages)
            {
                deliveryTimes[pkg.Id] = startTime + roundedOneWay;
                remainingPackages.Remove(pkg);
            }

            vehicle.ReserveForShipment(roundTripHours);
        }

        return deliveryTimes;
    }
}

