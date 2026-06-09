using System;

namespace PharmacyBenefitService.Models;

public class ManualReview
{
    public int Id { get; set; }
    public string PatientId { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string ErrorReason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
}
