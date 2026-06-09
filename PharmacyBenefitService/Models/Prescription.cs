namespace PharmacyBenefitService.Models;

public class Prescription
{
    public int Id { get; set; }
    public string PatientId { get; set; } = string.Empty;
    public string MedicationName { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}
