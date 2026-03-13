using CourierService.Domain;

namespace CourierService.Application;

/// <summary>
/// Selects shipments by evaluating all valid combinations
/// and choosing the one that:
/// 1. Maximizes number of packages
/// 2. If tie, maximizes total weight
/// 3. If tie, minimizes max distance (earliest deliverable)
/// </summary>
public sealed class ShipmentSelector
{
    public Shipment SelectBestShipment(
        IReadOnlyList<Package> availablePackages,
        Vehicle vehicle)
    {
        if (availablePackages == null) throw new ArgumentNullException(nameof(availablePackages));
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));

        var candidates = availablePackages.Where(p => p.WeightKg <= vehicle.MaxCarriableWeightKg).ToList();
        if (candidates.Count == 0)
            throw new InvalidOperationException("No packages can be carried by this vehicle.");

        Shipment? bestShipment = null;

        // Exhaustive subset enumeration with simple pruning by weight
        var n = candidates.Count;
        var bestCount = 0;
        var bestWeight = 0d;
        var bestMaxDistance = double.MaxValue;

        // Represent subsets by bitmask from 1..(2^n - 1)
        var totalSubsets = 1 << n;
        for (var mask = 1; mask < totalSubsets; mask++)
        {
            double weightSum = 0;
            int count = 0;
            double maxDistance = 0;
            var currentPackages = new List<Package>();

            for (var i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) == 0) continue;

                var pkg = candidates[i];
                weightSum += pkg.WeightKg;
                if (weightSum > vehicle.MaxCarriableWeightKg)
                {
                    currentPackages.Clear();
                    break; // prune overweight combination
                }

                count++;
                if (pkg.DistanceKm > maxDistance)
                {
                    maxDistance = pkg.DistanceKm;
                }

                currentPackages.Add(pkg);
            }

            if (currentPackages.Count == 0)
                continue;

            if (count > bestCount ||
                (count == bestCount && weightSum > bestWeight) ||
                (count == bestCount && Math.Abs(weightSum - bestWeight) < 1e-6 && maxDistance < bestMaxDistance))
            {
                bestCount = count;
                bestWeight = weightSum;
                bestMaxDistance = maxDistance;
                bestShipment = new Shipment(currentPackages);
            }
        }

        if (bestShipment == null)
            throw new InvalidOperationException("Failed to select a shipment.");

        return bestShipment;
    }
}

