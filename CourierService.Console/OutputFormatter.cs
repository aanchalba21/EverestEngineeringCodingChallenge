using CourierService.Application;

namespace CourierService.ConsoleApp;

public sealed class OutputFormatter
{
    public void WriteProblem1Results(
        TextWriter writer,
        IReadOnlyList<CostEstimationResult> results)
    {
        foreach (var result in results)
        {
            writer.WriteLine(
                $"{result.Package.Id} {result.DiscountAmount:0} {result.TotalCost:0}");
        }
    }

    public void WriteProblem2Results(
        TextWriter writer,
        IReadOnlyList<CostEstimationResult> results,
        IReadOnlyDictionary<string, double> deliveryTimes)
    {
        foreach (var result in results)
        {
            var time = deliveryTimes[result.Package.Id];
            var roundedTime = Math.Round(time, 2, MidpointRounding.AwayFromZero);
            writer.WriteLine(
                $"{result.Package.Id} {result.DiscountAmount:0} {result.TotalCost:0} {roundedTime:0.00}");
        }
    }
}

