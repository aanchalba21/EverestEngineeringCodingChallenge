namespace CourierService.Domain;

public sealed class CostBreakdown
{
    public decimal BaseCost { get; }
    public decimal WeightCost { get; }
    public decimal DistanceCost { get; }
    public decimal DiscountAmount { get; }
    public decimal TotalCost { get; }

    public CostBreakdown(
        decimal baseCost,
        decimal weightCost,
        decimal distanceCost,
        decimal discountAmount,
        decimal totalCost)
    {
        BaseCost = baseCost;
        WeightCost = weightCost;
        DistanceCost = distanceCost;
        DiscountAmount = discountAmount;
        TotalCost = totalCost;
    }
}

