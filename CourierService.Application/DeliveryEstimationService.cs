using CourierService.Domain;

namespace CourierService.Application;

public sealed class DeliveryEstimationService
{
    private readonly ShipmentSelector _shipmentSelector;
    private readonly DeliveryTimeEstimator _timeEstimator;

    public DeliveryEstimationService(ShipmentSelector shipmentSelector, DeliveryTimeEstimator timeEstimator)
    {
        _shipmentSelector = shipmentSelector ?? throw new ArgumentNullException(nameof(shipmentSelector));
        _timeEstimator = timeEstimator ?? throw new ArgumentNullException(nameof(timeEstimator));
    }

    public IDictionary<string, double> EstimateDeliveryTimes(
        IReadOnlyList<CostEstimationResult> costResults,
        int numberOfVehicles,
        double maxSpeedKmph,
        double maxCarriableWeightKg)
    {
        if (costResults == null) throw new ArgumentNullException(nameof(costResults));

        // validation: any package heavier than capacity is an input error
        var overweight = costResults
            .Select(r => r.Package)
            .Where(p => p.WeightKg > maxCarriableWeightKg)
            .Select(p => p.Id)
            .ToList();

        if (overweight.Count > 0)
        {
            throw new InputValidationException(
                $"One or more packages exceed max_carriable_weight: {string.Join(", ", overweight)}");
        }

        return _timeEstimator.EstimateDeliveryTimes(costResults, numberOfVehicles, maxSpeedKmph, maxCarriableWeightKg);
    }
}

