using System.ComponentModel.DataAnnotations;

namespace ClinicalTelemetryAPI.Models;

public class TelemetryReading
{
    public int Id { get; set; }
    
    [Required]
    public string PatientId { get; set; } = string.Empty;
    
    public int HeartRate { get; set; }
    
    public int BloodPressureSystolic { get; set; }
    
    public int BloodPressureDiastolic { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
