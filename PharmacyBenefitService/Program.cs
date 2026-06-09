using Microsoft.EntityFrameworkCore;
using PharmacyBenefitService;
using PharmacyBenefitService.Data;
using PharmacyBenefitService.Models;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<PharmacyDbContext>(options =>
    options.UseSqlite("Data Source=pharmacy.db"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Auto-apply migrations and seed initial queue
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PharmacyDbContext>();
    db.Database.Migrate();

    if (!db.PrescriptionQueue.Any())
    {
        db.PrescriptionQueue.AddRange(
            new Prescription { PatientId = "P101", MedicationName = "Amoxicillin", Cost = 25.50m },
            new Prescription { PatientId = "P102", MedicationName = "Humira", Cost = 6000.00m },
            new Prescription { PatientId = "P103", MedicationName = "Lisinopril", Cost = 15.00m },
            new Prescription { PatientId = "P104", MedicationName = "Oxycodone", Cost = 80.00m },
            new Prescription { PatientId = "P105", MedicationName = "Insulin Glargine", Cost = 350.00m }
        );
        db.SaveChanges();
    }
}

host.Run();
