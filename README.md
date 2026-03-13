### Courier Service Coding Challenge (.NET 8, C#)

This is a production-quality solution for the **Courier Service** coding challenge implemented as a .NET 8 C# console application.

The application solves:
- **Problem 1**: Delivery cost estimation with promotional offer rules
- **Problem 2**: Delivery time estimation with multiple vehicles and shipment scheduling

---

### Project Structure

- **`CourierService.Domain`**: Core domain models (`Package`, `OfferRule`, `Vehicle`, `Shipment`).
- **`CourierService.Application`**: Application services and business logic:
  - `OfferDiscountCalculator`
  - `CostEstimationService`
  - `ShipmentSelector`
  - `DeliveryTimeEstimator`
- **`CourierService.Console`**: Console UI, input parsing, and output formatting:
  - `InputParser`
  - `OutputFormatter`
  - `Program` (composition root)
- **`CourierService.Tests`**: xUnit test project with unit tests for discounts, cost calculation, shipment selection, and delivery time estimation.

---

### Assumptions

- **Input format**
  - First line: `base_delivery_cost no_of_packages`
  - Next `no_of_packages` lines: `pkg_id pkg_weight distance offer_code`
  - Optional final line (Problem 2): `no_of_vehicles max_speed max_carriable_weight`
  - If the final line is **missing**, only **Problem 1** (cost estimation) runs.
  - Values use standard invariant parsing (e.g. `.` as decimal separator).
- **Costs and discounts**
  - Base formula:  
    \[
    delivery\_cost = base\_delivery\_cost + (pkg\_weight \times 10) + (distance \times 5)
    \]
  - Discount is applied **only** when the package satisfies the matching offer rule exactly.
  - Discount amount is rounded to **nearest integer** using `MidpointRounding.AwayFromZero`.
  - Output `discount` and `total_cost` are printed as integer values.
- **Delivery time**
  - Time is measured in **hours**.
  - One-way travel time for a shipment is based on the **maximum distance** of packages in that shipment divided by `max_speed`.
  - For each package, `estimated_delivery_time = vehicle_start_time + one_way_travel_time`.
  - One-way travel time is rounded to 2 decimal places (away from zero) for output.
  - Vehicles become available again after a **round-trip**: `2 * one_way_travel_time`.
- **Offer rules**
  - Encoded exactly as given:
    - `OFR001`: 10% discount, distance 0–200, weight 70–200
    - `OFR002`: 7% discount, distance 50–150, weight 100–250
    - `OFR003`: 5% discount, distance 50–250, weight 10–150
  - Offer code comparisons are **case-insensitive**, but stored uppercase.

---

### Shipment Selection – Approaches Considered

We must, at each step, choose a shipment that:
1. Maximizes the **number of packages** in the shipment (subject to `max_carriable_weight`).
2. If tied, maximizes the **total weight**.
3. If still tied, chooses the shipment that can be **delivered first** (smallest max distance / one-way time).

**Approach 1 – Greedy by weight or distance**
- Sort packages (e.g. by weight descending or by distance) and pack greedily until capacity is full.
- **Pros**: Simple, fast (O(n log n)).
- **Cons**: Does **not guarantee optimality** for the given tie-breaking rules.
  - Can easily miss a combination with more packages or better total weight.
- **Fit**: Not acceptable because the challenge requires correct tie-breaking and optimality for each shipment.

**Approach 2 – Dynamic Programming (0/1 Knapsack style)**
- Treat weight as capacity and run a DP to maximize number of packages, then total weight, then minimize max distance.
- **Pros**: More systematic than greedy, can encode lexicographic objective in the state.
- **Cons**:
  - DP state becomes complex (needs to track both count and distance information, potentially multi-dimensional).
  - More code and edge cases, reducing readability and increasing bug surface for a coding challenge.
- **Fit**: Possible but **overly complex** for the small problem sizes expected in interviews.

**Approach 3 – Exhaustive Subset Enumeration with Pruning (Chosen)**
- For the current set of undelivered packages:
  - Enumerate all non-empty subsets using a bit mask.
  - For each subset, skip immediately if total weight exceeds `max_carriable_weight`.
  - Track:
    - package count
    - total weight
    - max distance (for earliest deliverable)
  - Choose the best subset using the lexicographic rules:
    1. higher package count
    2. higher total weight
    3. smaller max distance
- Complexity:
  - \(O(2^n \cdot n)\) per shipment selection step for `n` remaining packages.
  - For typical challenge sizes (small \(n\), e.g. ≤ 10–15), this is **very fast**.
- **Pros**:
  - Straightforward and easy to reason about.
  - Guarantees **correctness and optimality** per the problem statement.
  - Simple, small code, easy to test and review.
- **Cons**:
  - Exponential in the number of packages, but acceptable for interview-scale inputs.

**Chosen Approach**

The implementation uses **Approach 3 – Exhaustive Subset Enumeration with Pruning** inside `ShipmentSelector`.  
This offers:
- **Correctness first**: evaluates all valid combinations and applies the exact tie-breaking rules.
- **Readability**: a single, focused class with clear logic and comments.
- **Maintainability and testability**: easy to add more tests or tweak rules in one place.

For **large-scale production** (e.g. hundreds or thousands of packages per batch), we would:
- Replace the subset enumeration with a more scalable heuristic or DP-based approximation.
- Potentially run per-vehicle knapsack-like algorithms that:
  - maximize package count first,
  - then maximize weight,
  - and enforce a penalty for long-distance packages.
- Add metrics and profiling to balance optimality with performance under real-world constraints.

---

### High-Level Design

- **Domain layer**
  - `Package` encapsulates basic validation and normalization (id, weight, distance, offer code).
  - `OfferRule` contains the rule bounds and the method `IsApplicable` to test eligibility.
  - `Vehicle` tracks capacity, speed, and `NextAvailableTimeHours`.
  - `Shipment` represents a group of packages, precomputing total weight and max distance.

- **Application layer**
  - `OfferDiscountCalculator`:
    - Holds a collection of `OfferRule`.
    - Returns the discount percentage for a package or 0 if none applies.
  - `CostEstimationService`:
    - Implements the cost formula.
    - Uses `OfferDiscountCalculator` to compute discount, rounds the discount, and produces `CostEstimationResult`.
  - `ShipmentSelector`:
    - Implements the **exhaustive subset enumeration** logic described above.
  - `DeliveryTimeEstimator`:
    - Manages vehicles, selecting the **earliest available vehicle** each time.
    - Uses `ShipmentSelector` to choose a shipment from remaining packages.
    - Computes per-package delivery times and updates vehicle availability.

- **Console layer**
  - `InputParser`:
    - Reads from `TextReader` (console stdin in production, testable in unit tests).
    - Parses the input format and returns a strongly typed `ProblemInput`.
    - Supports both Problem 1 (no vehicle line) and Problem 2 (with vehicle line).
  - `OutputFormatter`:
    - Prints results strictly according to the required output format:
      - Problem 1: `pkg_id discount total_cost`
      - Problem 2: `pkg_id discount total_cost estimated_delivery_time`
  - `Program`:
    - Wires everything together:
      - Constructs offer rules, discount calculator, cost service.
      - Runs cost estimation for all packages.
      - If vehicle info is present:
        - Instantiates `ShipmentSelector` and `DeliveryTimeEstimator`.
        - Computes delivery times and prints Problem 2 output.
      - Otherwise prints Problem 1 output.

---

### Running the Application

From the repository root (`EverestEngineeringCoding Challenge`):

```bash
dotnet run --project CourierService.Console
```

You can then paste the full input into the console and press `Ctrl+D` (macOS/Linux) or `Ctrl+Z` followed by `Enter` (Windows) to signal end-of-input.

#### Example – Problem 1 Only

Input:

```text
100 3
PKG1 5 5 OFR001
PKG2 15 5 OFR002
PKG3 10 100 OFR003
```

Output (example format; specific numbers depend on offer eligibility):

```text
PKG1 <discount> <total_cost>
PKG2 <discount> <total_cost>
PKG3 <discount> <total_cost>
```

#### Example – Problem 1 + Problem 2

Input:

```text
100 3
PKG1 5 5 OFR001
PKG2 15 5 OFR002
PKG3 10 100 OFR003
2 70 200
```

Output format:

```text
PKG1 <discount> <total_cost> <estimated_delivery_time>
PKG2 <discount> <total_cost> <estimated_delivery_time>
PKG3 <discount> <total_cost> <estimated_delivery_time>
```

The exact numeric values depend on the offer applicability and scheduling outcome.

---

### Running Tests

From the repository root:

```bash
dotnet test
```

This runs the xUnit test suite in `CourierService.Tests`, covering:
- Valid discount scenarios
- Invalid / ineligible offer codes
- Shipment selection tie-breakers
- Vehicle scheduling and availability behavior
- Basic formatting and rounding behavior

---

### Error Handling & Validation

- The application validates:
  - Positive base cost, package weight, and number of packages.
  - Non-negative distances.
  - Positive vehicle counts, speeds, and capacities when provided.
- Parsing failures or missing input lines surface as clear errors written to `stderr` without crashing the runtime.

---

### Extensibility Notes

- Offer rules are centralized in `Program` and `OfferRule`, making it straightforward to:
  - Add or remove offers.
  - Move offer configuration to a file or configuration source.
- Shipment selection criteria are localized in `ShipmentSelector`, which can be:
  - Replaced with a more advanced heuristic for large-scale deployments.
  - Extended to consider priorities, SLAs, or time windows without affecting parsing or pricing logic.

