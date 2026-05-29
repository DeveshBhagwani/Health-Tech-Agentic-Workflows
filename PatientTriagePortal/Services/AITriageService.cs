using PatientTriagePortal.Models;

namespace PatientTriagePortal.Services;

public class AITriageService
{
    public (string TriageLevel, string Department) EvaluateSymptoms(string description)
    {
        var desc = description.ToLowerInvariant();
        
        // Mock Agentic AI logic based on keyword detection
        if (desc.Contains("chest pain") || desc.Contains("heart attack") || desc.Contains("stroke") || desc.Contains("breathing difficulty"))
        {
            return ("Emergency", "Cardiology / ER");
        }
        else if (desc.Contains("bone") || desc.Contains("fracture") || desc.Contains("broken"))
        {
            return ("Urgent", "Orthopedics");
        }
        else if (desc.Contains("fever") || desc.Contains("cough") || desc.Contains("headache") || desc.Contains("cold"))
        {
            return ("Routine", "General Practice");
        }
        else if (desc.Contains("stomach") || desc.Contains("nausea") || desc.Contains("vomiting"))
        {
            return ("Urgent", "Gastroenterology");
        }
        else if (desc.Contains("skin") || desc.Contains("rash"))
        {
            return ("Routine", "Dermatology");
        }
        
        // Default triage assignment
        return ("Routine", "General Practice");
    }
}
