using CourierService.Domain;

namespace CourierService.ConsoleApp;

public sealed class InputParser
{
    public ProblemInput Parse(TextReader reader)
    {
        if (reader == null) throw new ArgumentNullException(nameof(reader));

        var firstLine = ReadNonEmptyLine(reader);
        var firstParts = firstLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (firstParts.Length != 2)
            throw new InputParsingException("First line must contain base_delivery_cost and no_of_packages.");

        if (!decimal.TryParse(firstParts[0], out var baseDeliveryCost))
            throw new InputParsingException("Invalid base_delivery_cost.");
        if (!int.TryParse(firstParts[1], out var numberOfPackages) || numberOfPackages <= 0)
            throw new InputParsingException("Invalid no_of_packages.");

        var packages = new List<Package>();
        for (var i = 0; i < numberOfPackages; i++)
        {
            var line = ReadNonEmptyLine(reader);
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4)
                throw new InputParsingException("Package line must contain pkg_id, pkg_weight, distance, offer_code.");

            var id = parts[0];
            if (!double.TryParse(parts[1], out var weight))
                throw new InputParsingException("Invalid pkg_weight.");
            if (!double.TryParse(parts[2], out var distance))
                throw new InputParsingException("Invalid distance.");
            var offerCode = parts[3];

            packages.Add(new Package(id, weight, distance, offerCode));
        }

        // Try to read second problem line if present
        var rest = ReadRemainingNonEmptyLines(reader).ToList();
        int? numberOfVehicles = null;
        double? maxSpeed = null;
        double? maxCarriableWeight = null;

        if (rest.Count > 0)
        {
            var line = rest[0];
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                throw new InputParsingException("Second line must contain no_of_vehicles, max_speed, max_carriable_weight.");

            if (!int.TryParse(parts[0], out var vehicles) || vehicles <= 0)
                throw new InputParsingException("Invalid no_of_vehicles.");
            if (!double.TryParse(parts[1], out var speed) || speed <= 0)
                throw new InputParsingException("Invalid max_speed.");
            if (!double.TryParse(parts[2], out var maxWeight) || maxWeight <= 0)
                throw new InputParsingException("Invalid max_carriable_weight.");

            numberOfVehicles = vehicles;
            maxSpeed = speed;
            maxCarriableWeight = maxWeight;
        }

        return new ProblemInput(baseDeliveryCost, packages, numberOfVehicles, maxSpeed, maxCarriableWeight);
    }

    private static string ReadNonEmptyLine(TextReader reader)
    {
        string? line;
        do
        {
            line = reader.ReadLine();
            if (line == null)
                throw new InputParsingException("Unexpected end of input.");
        } while (string.IsNullOrWhiteSpace(line));

        return line;
    }

    private static IEnumerable<string> ReadRemainingNonEmptyLines(TextReader reader)
    {
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                yield return line;
            }
        }
    }
}

