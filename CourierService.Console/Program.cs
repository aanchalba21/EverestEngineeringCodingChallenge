using CourierService.Application;
using CourierService.ConsoleApp;
using CourierService.Domain;

var parser = new InputParser();
var formatter = new OutputFormatter();

try
{
    var input = parser.Parse(Console.In);

    var offerRules = new[]
    {
        new OfferRule("OFR001", 10, 0, 200, 70, 200),
        new OfferRule("OFR002", 7, 50, 150, 100, 250),
        new OfferRule("OFR003", 5, 50, 250, 10, 150)
    };

    var costCalculator = new PackageCostCalculator(offerRules);
    var costService = new CostEstimationService(costCalculator);

    var costResults = input.Packages
        .Select(p => costService.EstimateCost(input.BaseDeliveryCost, p))
        .ToList();

    if (input.NumberOfVehicles is null ||
        input.MaxSpeedKmph is null ||
        input.MaxCarriableWeightKg is null)
    {
        formatter.WriteProblem1Results(Console.Out, costResults);
    }
    else
    {
        var shipmentSelector = new ShipmentSelector();
        var timeEstimator = new DeliveryTimeEstimator(shipmentSelector);
        var deliveryService = new DeliveryEstimationService(shipmentSelector, timeEstimator);

        var deliveryTimes = deliveryService.EstimateDeliveryTimes(
            costResults,
            input.NumberOfVehicles.Value,
            input.MaxSpeedKmph.Value,
            input.MaxCarriableWeightKg.Value);

        formatter.WriteProblem2Results(Console.Out, costResults, (IReadOnlyDictionary<string, double>)deliveryTimes);
    }
}
catch (InputParsingException ex)
{
    Console.Error.WriteLine($"Input parsing error: {ex.Message}");
}
catch (InputValidationException ex)
{
    Console.Error.WriteLine($"Input validation error: {ex.Message}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
}
