using Domain.Memberships;

namespace Presentation.WebApp.Models;

public sealed class MyMembershipViewModel
{
    public bool HasActiveMembership => Active is not null;

    public ActiveMembershipViewModel? Active { get; init; }

    public sealed record ActiveMembershipViewModel(
        MembershipTier Tier,
        decimal MonthlyPrice,
        int DurationInMonths,
        DateTime StartUtc,
        DateTime EndUtc,
        MembershipSubscriptionStatus Status);
}