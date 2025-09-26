namespace Insurance.Domain;

public enum ClaimStatus { Submitted, Validated, Rejected, Paid }

public sealed class Policy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PolicyNumber { get; set; } = default!;
    public string HolderName { get; set; } = default!;
    public DateOnly EffectiveDate { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public decimal Deductible { get; set; }
    public decimal CoverageLimit { get; set; }
}

public sealed class Claim
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PolicyNumber { get; set; } = default!;
    public DateOnly LossDate { get; set; }
    public decimal Amount { get; set; }
    public string LossType { get; set; } = "Property";
    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public List<ClaimEvent> Events { get; set; } = new();
}

public sealed class ClaimEvent
{
    public long Id { get; set; }
    public Guid ClaimId { get; set; }
    public string Type { get; set; } = default!;
    public string? Reason { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}