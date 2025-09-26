namespace Insurance.Domain;

public interface IClaimValidationService
{
    (bool ok, string? reason) Validate(Policy policy, Claim claim);
}

public sealed class SimpleClaimValidationService : IClaimValidationService
{
    public (bool ok, string? reason) Validate(Policy p, Claim c)
    {
        if (c.LossDate < p.EffectiveDate || c.LossDate > p.ExpirationDate)
            return (false, "Loss date outside policy term.");
        if (c.Amount <= 0) return (false, "Amount must be positive.");
        if (c.Amount < p.Deductible) return (false, "Below deductible.");
        if (c.Amount > p.CoverageLimit) return (false, "Exceeds coverage limit.");
        return (true, null);
    }
}