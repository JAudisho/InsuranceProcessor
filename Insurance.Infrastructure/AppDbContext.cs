using Insurance.Domain;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimEvent> ClaimEvents => Set<ClaimEvent>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Policy>().HasIndex(x => x.PolicyNumber).IsUnique();
        b.Entity<Claim>().HasIndex(x => new { x.PolicyNumber, x.CreatedUtc });
        b.Entity<ClaimEvent>().HasIndex(x => new { x.ClaimId, x.CreatedUtc });

        b.Entity<Claim>()
            .HasMany(c => c.Events)
            .WithOne()
            .HasForeignKey(e => e.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}