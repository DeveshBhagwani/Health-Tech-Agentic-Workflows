using Microsoft.EntityFrameworkCore;
using PatientTriagePortal.Models;

namespace PatientTriagePortal.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<PatientSymptom> PatientSymptoms { get; set; }
}
