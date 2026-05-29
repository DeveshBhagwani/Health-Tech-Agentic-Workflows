# Healthcare Enterprise Automation Suite

This repository contains a suite of three cloud-native microservices designed to automate, streamline, and scale healthcare processes. The ecosystem demonstrates a modern architectural approach integrating .NET 8, Python, and enterprise engineering patterns (Observability, Resilience, Scalability, and Security).

---

## 1. PatientTriagePortal

**Purpose**: 
To automate initial patient intake by collecting symptom descriptions and intelligently triaging patients into the appropriate clinical department based on severity.

**Design & Features**:
- **Strict MVC Architecture**: Decouples the presentation layer (Bootstrap 5 HTML forms and dashboards) from the business logic.
- **AI Triage Simulation**: A dedicated `AITriageService` scans input for keywords indicating emergency, urgent, or routine conditions.
- **Observability (Serilog)**: Implements structured logging across the application, tracking AI decisions and user submissions both to the console and daily rolling text files.
- **Automated Testing (xUnit)**: Incorporates a standalone testing project to ensure the `AITriageService` consistently routes critical symptoms (e.g., chest pain) to the ER.

**Tech Stack**:
- C# / .NET 8 / ASP.NET Core MVC
- Entity Framework Core (In-Memory)
- Serilog (Structured Logging)
- xUnit (Unit Testing)
- Bootstrap 5

---

## 2. ClinicalTelemetryAPI

**Purpose**: 
To serve as a high-throughput ingestion and querying backend for continuous clinical patient data (e.g., heart rate, blood pressure).

**Design & Features**:
- **Scalable REST API**: A strictly versioned API providing endpoints for both data ingestion (`POST`) and aggregate querying (`GET`).
- **Data Seeding**: Automatically populates the database on startup with 100 realistic mock vitals across multiple patients spanning the last 48 hours.
- **Performance Caching**: Integrates `IMemoryCache` on the summary endpoint to offload the database and rapidly serve 24-hour calculated averages (cached for 60 seconds).
- **API Versioning & Documentation**: Employs `Asp.Versioning.Mvc` and `Swashbuckle.AspNetCore` (Swagger) to ensure backward compatibility and auto-generate OpenAPI documentation.

**Tech Stack**:
- C# / .NET 8 / ASP.NET Core Web API
- Entity Framework Core (SQLite)
- `IMemoryCache` for performance
- Swagger / OpenAPI for documentation
- GitHub Actions for CI/CD

---

## 3. PharmacyBenefitService

**Purpose**: 
To evaluate and automatically process pending pharmacy prescriptions utilizing a polyglot microservice pattern.

**Design & Features**:
- **Hybrid Polyglot Execution**: A C# .NET Worker Service acts as a high-performance message queue orchestrator, while a standalone Python script acts as the flexible clinical rules engine.
- **Interprocess Communication**: The C# Worker serializes patient prescriptions to JSON, passes them to the Python CLI via `System.Diagnostics.Process`, and deserializes the stdout JSON response to determine the final approval status.
- **Resilience (Polly)**: The interprocess call to Python is wrapped in an asynchronous Polly retry policy, ensuring that transient faults or engine crashes are retried seamlessly.
- **Secrets Management**: Connects to the configuration layer (`appsettings.json`) to securely retrieve a simulated `InsuranceAPIKey`, demonstrating environment-aware security practices.

**Tech Stack**:
- C# / .NET 8 / Background Worker Service
- Python 3 (Standalone Rules Engine)
- Polly (Transient Fault Handling / Retry Policies)
- System.Text.Json (Cross-boundary serialization)
