using Domain.Common;

namespace Domain.Memberships;

public enum MembershipSubscriptionStatus
{
    Active = 1,
    Cancelled = 2,
    Expired = 3
}

public class MembershipSubscription
{
    private MembershipSubscription() { }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public Guid MembershipOfferId { get; private set; }

    public int DurationInMonths { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }

    public MembershipSubscriptionStatus Status { get; private set; }
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;

    public static MembershipSubscription Create(
        Guid id,
        string userId,
        Guid membershipOfferId,
        int durationInMonths,
        DateTime startUtc,
        DateTime createdUtc)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId is required.");

        if (membershipOfferId == default)
            throw new DomainException("MembershipOfferId is required.");

        if (durationInMonths < 1)
            throw new DomainException("Duration must be at least 1 month.");

        if (startUtc == default)
            startUtc = DateTime.UtcNow;

        return new MembershipSubscription
        {
            Id = id == default ? Guid.NewGuid() : id,
            UserId = userId.Trim(),
            MembershipOfferId = membershipOfferId,
            DurationInMonths = durationInMonths,
            StartUtc = startUtc,
            EndUtc = startUtc.AddMonths(durationInMonths),
            Status = MembershipSubscriptionStatus.Active,
            CreatedUtc = createdUtc == default ? DateTime.UtcNow : createdUtc
        };
    }

    public bool IsActiveAt(DateTime utcNow)
        => Status == MembershipSubscriptionStatus.Active && EndUtc > utcNow;

    public void Cancel(DateTime cancelledUtc)
    {
        if (Status != MembershipSubscriptionStatus.Active)
            throw new DomainException("Only active subscriptions can be cancelled.");

        Status = MembershipSubscriptionStatus.Cancelled;
        EndUtc = cancelledUtc;
    }
}