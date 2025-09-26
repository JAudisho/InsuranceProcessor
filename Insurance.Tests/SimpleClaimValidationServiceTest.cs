using Insurance.Domain;
using Xunit;

public class SimpleClaimValidationServiceTests
{
    private static Policy SamplePolicy() => new Policy
    {
        PolicyNumber = "PC-10001",
        HolderName = "Jane Doe",
        EffectiveDate = new DateOnly(2025, 1, 1),
        ExpirationDate = new DateOnly(2025, 12, 31),
        Deductible = 500m,
        CoverageLimit = 25000m
    };

    [Fact]
    public void Valid_claim_passes()
    {
        var svc = new SimpleClaimValidationService();
        var policy = SamplePolicy();
        var claim = new Claim { PolicyNumber = policy.PolicyNumber, LossDate = new DateOnly(2025, 5, 20), Amount = 1200m };
        var (ok, reason) = svc.Validate(policy, claim);
        Assert.True(ok);
        Assert.Null(reason);
    }

    [Fact]
    public void Below_deductible_fails()
    {
        var svc = new SimpleClaimValidationService();
        var policy = SamplePolicy();
        var claim = new Claim { PolicyNumber = policy.PolicyNumber, LossDate = new DateOnly(2025, 6, 1), Amount = 300m };
        var (ok, reason) = svc.Validate(policy, claim);
        Assert.False(ok);
        Assert.Equal("Below deductible.", reason);
    }

    [Fact]
    public void Over_limit_fails()
    {
        var svc = new SimpleClaimValidationService();
        var policy = SamplePolicy();
        var claim = new Claim { PolicyNumber = policy.PolicyNumber, LossDate = new DateOnly(2025, 6, 1), Amount = 50000m };
        var (ok, reason) = svc.Validate(policy, claim);
        Assert.False(ok);
        Assert.Equal("Exceeds coverage limit.", reason);
    }

    [Fact]
    public void Out_of_term_fails()
    {
        var svc = new SimpleClaimValidationService();
        var policy = SamplePolicy();
        var claim = new Claim { PolicyNumber = policy.PolicyNumber, LossDate = new DateOnly(2026, 1, 1), Amount = 1200m };
        var (ok, reason) = svc.Validate(policy, claim);
        Assert.False(ok);
        Assert.Equal("Loss date outside policy term.", reason);
    }

    [Fact]
    public void Non_positive_amount_fails()
    {
        var svc = new SimpleClaimValidationService();
        var policy = SamplePolicy();
        var claim = new Claim { PolicyNumber = policy.PolicyNumber, LossDate = new DateOnly(2025, 5, 20), Amount = 0m };
        var (ok, reason) = svc.Validate(policy, claim);
        Assert.False(ok);
        Assert.Equal("Amount must be positive.", reason);
    }
}