using System.ComponentModel.DataAnnotations;

namespace PatientTriagePortal.Models;

public class PatientSymptom
{
    public int Id { get; set; }
    
    [Required]
    public string PatientName { get; set; } = string.Empty;
    
    [Required]
    public string SymptomDescription { get; set; } = string.Empty;
    
    public string TriageLevel { get; set; } = string.Empty;
    
    public string SuggestedDepartment { get; set; } = string.Empty;
    
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
