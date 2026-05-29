# Pharmacy Benefit Service

A polyglot microservice system demonstrating end-to-end healthcare insurance automation using C# and Python.

## Architecture & Integration
This project utilizes a hybrid approach:
- **C# .NET Worker Service**: Simulates a high-throughput enterprise queue processing pending prescriptions.
- **Python Rules Engine**: A standalone, highly flexible script (`insurance_rules.py`) that acts as the clinical decision-maker.

The integration is achieved by the C# Worker Service directly invoking the Python engine via `System.Diagnostics.Process`. The .NET service serializes the prescription object into JSON, passes it as a command-line argument to the Python script, and cleanly captures standard output (stdout) for deserialization back into C#.

## Features
- **In-Memory Queue Simulation**: Evaluates a batch of 5 pending prescriptions sequentially.
- **Dynamic Rules Engine**: 
  - Automatically denies controlled substances (e.g., Oxycodone).
  - Flags high-cost medications (>$500) as `Pending` for manual review (e.g., Humira).
  - Automatically approves standard Tier 1 medications.
- **Polyglot Communication**: Seamless JSON serialization and deserialization across the process boundary.

## How to Run Locally
1. Ensure the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and [Python 3](https://www.python.org/downloads/) are installed on your machine.
2. Ensure `python` is added to your environment `PATH`.
3. Open a terminal in the `PharmacyBenefitService` directory.
4. Execute the worker service:
   ```bash
   dotnet run
   ```
5. Watch the console to see the C# service orchestrate approvals by securely exchanging JSON with the underlying Python rules engine!
