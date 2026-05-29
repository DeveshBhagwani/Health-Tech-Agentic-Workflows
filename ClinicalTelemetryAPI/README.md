# ClinicalTelemetryAPI

A highly scalable .NET Core Web API for ingesting, persisting, and querying patient clinical telemetry data.

## API Design & Features
- **POST `/api/telemetry`**: Accepts clinical readings (Heart Rate, Systolic/Diastolic Blood Pressure) and securely saves them to the database.
- **GET `/api/telemetry/{patientId}/summary`**: Executes a highly optimized LINQ query directly at the database level to fetch average patient vitals strictly over the last 24 hours.
- **Automatic DB Seeding**: The API initializes a SQLite database on startup and seeds 100 realistic mock records spread across the last 48 hours for patients `P001`, `P002`, and `P003`.

## Tech Stack
- **Framework**: C# .NET 8 Web API
- **ORM / Data Access**: Entity Framework Core
- **Database**: SQLite (Zero-configuration file-based DB, perfect for containerized microservices and local testing)

## Continuous Integration
A standard GitHub Actions pipeline is included (`.github/workflows/build.yml`). This ensures that every push to the `main` branch automatically restores dependencies and validates the build using the `ubuntu-latest` runner.

## How to Run Locally
1. Ensure the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) is installed.
2. Open your terminal in the `ClinicalTelemetryAPI` folder.
3. Run the application:
   ```bash
   dotnet run
   ```
4. Test the Endpoints using cURL, Postman, or Thunder Client:
   - **Get Summary**: `GET http://localhost:5000/api/telemetry/P001/summary` (Replace `P001` with `P002` or `P003` to view other patient data).
   - **Post Data**:
     ```json
     POST http://localhost:5000/api/telemetry
     {
       "patientId": "P001",
       "heartRate": 85,
       "bloodPressureSystolic": 125,
       "bloodPressureDiastolic": 80
     }
     ```
