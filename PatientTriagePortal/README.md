# Patient Triage Portal

A complete, runnable ASP.NET Core MVC application demonstrating a mocked Agentic AI/LLM integration for healthcare automation. 
Patients submit their symptoms via a web form, and an AI service analyzes the text to determine the triage severity level and appropriate medical department.

## Tech Stack
- **Framework**: .NET 8 / ASP.NET Core MVC
- **Database**: Entity Framework Core (In-Memory Database for zero-setup execution)
- **Frontend**: HTML5, CSS3, Bootstrap 5

## Architecture Highlights
- **Strict MVC Pattern**: Clean separation between `Models`, `Views`, and `Controllers`.
- **Service Layer**: An isolated `AITriageService` mimics LLM text-scanning and keyword extraction to generate structured JSON-like routing for the patient.
- **Responsive UI**: A Bootstrap 5-powered frontend ensures the application looks great on all devices.

## How to Run
1. Ensure you have the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your machine.
2. Open your terminal or command prompt.
3. Navigate to the project directory: `cd PatientTriagePortal`
4. Execute the following command to run the application:
   ```bash
   dotnet run
   ```
5. Open your web browser and navigate to the URL provided in the terminal (usually `http://localhost:5000` or `https://localhost:5001`).

## Application Flow
1. **Patient Intake (`/Triage/Index`)**: Patients fill out a form with their name and a description of their symptoms.
2. **AI Simulation (`AITriageService.cs`)**: The mock AI agent scans the input. 
   - Keywords like "chest pain" or "stroke" route the patient to **Emergency (Cardiology / ER)**.
   - Keywords like "fracture" or "broken" route the patient to **Urgent (Orthopedics)**.
   - Keywords like "fever" route the patient to **Routine (General Practice)**.
3. **Dashboard (`/Triage/Dashboard`)**: A dynamic dashboard view lists all patient submissions, their assigned triage levels (color-coded), and departments.

Enjoy testing the Patient Triage Portal!
