using Domain.Common;

namespace Domain.Memberships;

public enum MembershipTier
{
    Standard = 1,
    Premium = 2
}

public class MembershipOffer
{
    private MembershipOffer() { }

    public Guid Id { get; private set; }
    public MembershipTier Tier { get; private set; }
    public decimal MonthlyPrice { get; private set; }
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;

    public static MembershipOffer Create(Guid id, MembershipTier tier, decimal monthlyPrice, DateTime createdUtc)
    {
        if (!Enum.IsDefined(typeof(MembershipTier), tier))
            throw new DomainException("Invalid membership tier.");

        if (monthlyPrice < 0)
            throw new DomainException("Monthly price cannot be negative.");

        return new MembershipOffer
        {
            Id = id == default ? Guid.NewGuid() : id,
            Tier = tier,    
            MonthlyPrice = monthlyPrice,
            CreatedUtc = createdUtc == default ? DateTime.UtcNow : createdUtc
        };
    }
}
