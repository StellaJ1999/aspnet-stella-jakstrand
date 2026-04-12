using Domain.Memberships;

namespace Presentation.WebApp.Models;

public sealed class MembershipJoinViewModel
{
    public sealed record OfferOption(MembershipTier Tier, decimal MonthlyPrice);

    public IReadOnlyList<OfferOption> Offers { get; init; } = Array.Empty<OfferOption>();

    public MembershipTier Tier { get; set; } = MembershipTier.Standard;
    public int DurationInMonths { get; set; } = 1;

    public string? ErrorMessage { get; set; }

    public decimal MonthlyPrice => Offers.FirstOrDefault(x => x.Tier == Tier)?.MonthlyPrice ?? 0m;
    public decimal TotalPrice => MonthlyPrice * DurationInMonths;
}