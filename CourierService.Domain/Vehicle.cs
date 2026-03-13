namespace CourierService.Domain;

public sealed class Vehicle
{
    public int Id { get; }
    public double MaxSpeedKmph { get; }
    public double MaxCarriableWeightKg { get; }

    public double NextAvailableTimeHours { get; private set; }

    public Vehicle(int id, double maxSpeedKmph, double maxCarriableWeightKg)
    {
        if (maxSpeedKmph <= 0) throw new ArgumentOutOfRangeException(nameof(maxSpeedKmph));
        if (maxCarriableWeightKg <= 0) throw new ArgumentOutOfRangeException(nameof(maxCarriableWeightKg));

        Id = id;
        MaxSpeedKmph = maxSpeedKmph;
        MaxCarriableWeightKg = maxCarriableWeightKg;
        NextAvailableTimeHours = 0;
    }

    public void ReserveForShipment(double shipmentRoundTripHours)
    {
        if (shipmentRoundTripHours < 0)
            throw new ArgumentOutOfRangeException(nameof(shipmentRoundTripHours));

        NextAvailableTimeHours += shipmentRoundTripHours;
    }
}

