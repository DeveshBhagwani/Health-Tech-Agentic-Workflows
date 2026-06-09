using System;

namespace PharmacyBenefitService.Models;

public class ClaimsAudit
{
    public int Id { get; set; }
    public string PatientId { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}
