using CourierService.Application;

namespace CourierService.ConsoleApp;

public sealed class OutputFormatter
{
    public void WriteProblem1Results(
        TextWriter writer,
        IReadOnlyList<CostEstimationResult> results)
    {
        const string pkgIdHeader = "PKG_ID";
        const string discountHeader = "DISCOUNT";
        const string totalCostHeader = "TOTAL_COST";

        var idWidth = Math.Max(pkgIdHeader.Length, results.Count == 0 ? 0 : results.Max(r => r.Package.Id.Length));
        var discountStrings = results.Select(r => $"₹{r.DiscountAmount:0}").ToList();
        var totalCostStrings = results.Select(r => $"₹{r.TotalCost:0}").ToList();

        var discountWidth = Math.Max(discountHeader.Length, discountStrings.Count == 0 ? 0 : discountStrings.Max(s => s.Length));
        var totalCostWidth = Math.Max(totalCostHeader.Length, totalCostStrings.Count == 0 ? 0 : totalCostStrings.Max(s => s.Length));

        writer.WriteLine(
            $"{pkgIdHeader.PadRight(idWidth)} {discountHeader.PadLeft(discountWidth)} {totalCostHeader.PadLeft(totalCostWidth)}");

        for (var i = 0; i < results.Count; i++)
        {
            var result = results[i];
            var id = result.Package.Id.PadRight(idWidth);
            var discount = discountStrings[i].PadLeft(discountWidth);
            var totalCost = totalCostStrings[i].PadLeft(totalCostWidth);
            writer.WriteLine($"{id} {discount} {totalCost}");
        }
    }

    public void WriteProblem2Results(
        TextWriter writer,
        IReadOnlyList<CostEstimationResult> results,
        IReadOnlyDictionary<string, double> deliveryTimes)
    {
        const string pkgIdHeader = "PKG_ID";
        const string discountHeader = "DISCOUNT";
        const string totalCostHeader = "TOTAL_COST";
        const string timeHeader = "ESTIMATED_DELIVERY_TIME";

        var idWidth = Math.Max(pkgIdHeader.Length, results.Count == 0 ? 0 : results.Max(r => r.Package.Id.Length));
        var discountStrings = results.Select(r => $"₹{r.DiscountAmount:0}").ToList();
        var totalCostStrings = results.Select(r => $"₹{r.TotalCost:0}").ToList();

        var discountWidth = Math.Max(discountHeader.Length, discountStrings.Count == 0 ? 0 : discountStrings.Max(s => s.Length));
        var totalCostWidth = Math.Max(totalCostHeader.Length, totalCostStrings.Count == 0 ? 0 : totalCostStrings.Max(s => s.Length));

        var timeStrings = results
            .Select(r =>
            {
                var time = deliveryTimes[r.Package.Id];
                var roundedTime = Math.Round(time, 2, MidpointRounding.AwayFromZero);
                return $"{roundedTime:0.00} hours";
            })
            .ToList();
        var timeWidth = Math.Max(timeHeader.Length, timeStrings.Count == 0 ? 0 : timeStrings.Max(t => t.Length));

        writer.WriteLine(
            $"{pkgIdHeader.PadRight(idWidth)} {discountHeader.PadLeft(discountWidth)} {totalCostHeader.PadLeft(totalCostWidth)} {timeHeader.PadLeft(timeWidth)}");

        for (var i = 0; i < results.Count; i++)
        {
            var result = results[i];
            var id = result.Package.Id.PadRight(idWidth);
            var discount = discountStrings[i].PadLeft(discountWidth);
            var totalCost = totalCostStrings[i].PadLeft(totalCostWidth);
            var time = timeStrings[i].PadLeft(timeWidth);

            writer.WriteLine($"{id} {discount} {totalCost} {time}");
        }
    }
}

