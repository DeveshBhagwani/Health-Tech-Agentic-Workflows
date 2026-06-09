using Microsoft.EntityFrameworkCore;
using PharmacyBenefitService.Models;

namespace PharmacyBenefitService.Data;

public class PharmacyDbContext : DbContext
{
    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options) { }
    
    public DbSet<Prescription> PrescriptionQueue { get; set; }
    public DbSet<ManualReview> ManualReviews { get; set; }
    public DbSet<ClaimsAudit> ClaimsAudits { get; set; }
}
