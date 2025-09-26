using Insurance.Domain;
using Insurance.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

builder.Services.AddScoped<IClaimValidationService, SimpleClaimValidationService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Seed a sample policy
app.MapPost("/api/policies/seed", async (AppDbContext db) =>
{
    if (!await db.Policies.AnyAsync())
    {
        db.Policies.Add(new Policy
        {
            PolicyNumber = "PC-10001",
            HolderName = "Jane Doe",
            EffectiveDate = new DateOnly(2025, 1, 1),
            ExpirationDate = new DateOnly(2025, 12, 31),
            Deductible = 500,
            CoverageLimit = 25000
        });
        await db.SaveChangesAsync();
    }
    return Results.Ok();
});

// Create a claim
app.MapPost("/api/claims", async (AppDbContext db, CreateClaimDto dto) =>
{
    var claim = new Claim
    {
        PolicyNumber = dto.PolicyNumber,
        LossDate = dto.LossDate,
        Amount = dto.Amount,
        LossType = dto.LossType ?? "Property"
    };
    claim.Events.Add(new ClaimEvent { ClaimId = claim.Id, Type = "SUBMITTED" });

    db.Claims.Add(claim);
    await db.SaveChangesAsync();
    return Results.Created($"/api/claims/{claim.Id}", new { claim.Id });
});

// Validate a claim against policy rules
app.MapPost("/api/claims/{id:guid}/validate", async (Guid id, AppDbContext db, IClaimValidationService svc) =>
{
    var claim = await db.Claims.Include(c => c.Events).FirstOrDefaultAsync(c => c.Id == id);
    if (claim is null) return Results.NotFound();

    var policy = await db.Policies.FirstOrDefaultAsync(p => p.PolicyNumber == claim.PolicyNumber);
    if (policy is null)
    {
        claim.Status = ClaimStatus.Rejected;
        claim.Events.Add(new ClaimEvent { ClaimId = claim.Id, Type = "REJECTED", Reason = "Unknown policy" });
        await db.SaveChangesAsync();
        return Results.Ok(new { claim.Status, reason = "Unknown policy" });
    }

    var (ok, reason) = svc.Validate(policy, claim);
    if (ok)
    {
        claim.Status = ClaimStatus.Validated;
        claim.Events.Add(new ClaimEvent { ClaimId = claim.Id, Type = "VALIDATED" });
    }
    else
    {
        claim.Status = ClaimStatus.Rejected;
        claim.Events.Add(new ClaimEvent { ClaimId = claim.Id, Type = "REJECTED", Reason = reason });
    }

    await db.SaveChangesAsync();
    return Results.Ok(new { claim.Status, reason });
});

// Paged list
app.MapGet("/api/claims", async (AppDbContext db, int page = 1, int pageSize = 20, string? policy = null) =>
{
    var q = db.Claims.AsNoTracking();
    if (!string.IsNullOrWhiteSpace(policy))
        q = q.Where(c => c.PolicyNumber == policy);

    var total = await q.CountAsync();
    var items = await q.OrderByDescending(c => c.CreatedUtc)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(c => new ClaimListItemDto(c.Id, c.PolicyNumber, c.Amount, c.Status, c.CreatedUtc))
        .ToListAsync();

    return Results.Ok(new { total, items });
});

app.Run();

record CreateClaimDto(string PolicyNumber, DateOnly LossDate, decimal Amount, string? LossType);
record ClaimListItemDto(Guid Id, string PolicyNumber, decimal Amount, ClaimStatus Status, DateTime CreatedUtc);