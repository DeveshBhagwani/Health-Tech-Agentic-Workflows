using Microsoft.EntityFrameworkCore;
using ClinicalTelemetryAPI.Models;

namespace ClinicalTelemetryAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TelemetryReading> TelemetryReadings { get; set; }

    public void SeedData()
    {
        Database.EnsureCreated();
        
        if (!TelemetryReadings.Any())
        {
            var random = new Random();
            var patientIds = new[] { "P001", "P002", "P003" };
            var readings = new List<TelemetryReading>();
            var now = DateTime.UtcNow;

            for (int i = 0; i < 100; i++)
            {
                var patientId = patientIds[random.Next(patientIds.Length)];
                // Random time within the last 48 hours
                var timeOffsetHours = random.NextDouble() * 48;
                
                readings.Add(new TelemetryReading
                {
                    PatientId = patientId,
                    HeartRate = random.Next(60, 100),
                    BloodPressureSystolic = random.Next(110, 140),
                    BloodPressureDiastolic = random.Next(70, 90),
                    Timestamp = now.AddHours(-timeOffsetHours)
                });
            }

            TelemetryReadings.AddRange(readings);
            SaveChanges();
        }
    }
}
